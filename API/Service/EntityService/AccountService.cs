using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Service.ServiceInterfaces.IStrategy;
using MyLife.Shared.Accident;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;

namespace MyLife.Service.EntityService
{
    public class AccountService(
        AppStorage storage,
        SubscriptionService subscriptionService,
        FileService fileService,
        IEnumerable<IAccountUploadStrategy> uploadStrategies,
        IEnumerable<IAccountUpdateStrategy> updateStrategies,
        AccountMapper accountMapper,
        IOptions<JwtOption> options
        )
    {
        private ServiceStatus UploadCheck(AccountDto dto)
        {
            return uploadStrategies.Select(strategy => strategy.Check(dto)).FirstOrDefault(result => !result.IsValid) is ServiceStatus { IsValid: false, Message: string message }
                ? new ServiceStatus(message, false)
                : new ServiceStatus(string.Empty);
        }

        private ServiceStatus UpdateCheck(AccountEntity account)
        {
            return updateStrategies.Select(strategy => strategy.Check(account)).FirstOrDefault(result => !result.IsValid) is ServiceStatus { IsValid: false, Message: string message }
                ? new ServiceStatus(message, false)
                : new ServiceStatus(string.Empty);
        }

        private readonly bool InitState = !storage.Account.Any(e => e.IsValid);

        [Obsolete("暂不允许提供到Controller")]
        public async Task<IEnumerable<AccountEntity>> TryReadListAsync()
            => await storage.Account.Include(e => e.Subscriptions).AsNoTracking().Where(a => a.IsValid).ToListAsync();

        public async Task<AccountEntity?> TryReadAsync(Guid? id = null)
            => await storage.Account.Include(e => e.Subscriptions).AsNoTracking().FirstOrDefaultAsync(a => id == null ? a.IsValid : a.UID == id);

        public async Task<AccountEntity> TryCreateAsync(AccountDto account)
        {
            var status = UploadCheck(account);
            if (!status.IsValid)
                throw new OperateTransactionFailedException(status.Message);

            AccountEntity user = accountMapper.ToEntity(account);
            storage.Account.Add(user);
            return user;
        }

        /// <summary>
        /// 仅用于更新账户名
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="OperateTransactionFailedException"></exception>
        public async Task<Guid> TryUpdateAsync(AccountDto dto, Guid guid)
        {
            var transaction = await storage.Database.BeginTransactionAsync();
            try
            {
                AccountEntity account = await TryReadAsync(guid) ?? throw new OperateTransactionFailedException("账户不存在"); ;// 此处不可能为null，因为前面已经检查过了

                var entites = await subscriptionService.TryUpdateAsync(account.UID, dto.Subscriptions);
                accountMapper.UpdateEntity(dto, account);
                UpdateCheck(account);
                storage.Entry(account).State = EntityState.Modified;
                // account.Subscriptions = (ICollection<SubscriptionEntity>)entites;
                //subscriptionMapper.UpdateEntity(, account.Subscriptions); 未经安全检查的风险方法
                // storage.Account.Update(account);// 因为 TryReadAsync 中使用了 AsNoTracking，所以这里需要显式调用 Update 来告诉 EF Core 这个实体需要被更新。
                await storage.SaveChangesAsync();
                await transaction.CommitAsync();

                return account.UID;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> TryDeleteAsync(Guid guid)
        {
            int removedCount = 0;
            var account = await TryReadAsync(guid) ?? throw new OperateTransactionFailedException("账户不存在");

            if (account.Avatar is Guid avatar)
                removedCount += await fileService.TryDeleteAsync(avatar);// 删除头像
            var subs = account.Subscriptions.Select(s => s.SubscriptionIcon).ToList();
            subs.ForEach(async e => removedCount += await fileService.TryDeleteAsync(e));
            // 删除订阅
            storage.Account.Remove(account);// 按照设计 删除账户会级联删除订阅，因为 SubscriptionEntity 中的 Account 导航属性被配置为 Cascade Delete。
            return (removedCount += await storage.SaveChangesAsync());
        }

        public async Task<bool> TryLogin(LoginDto dto)
            => dto.Password == options.Value.SecretKey && await storage.Account.AsNoTracking().FirstOrDefaultAsync(a => a.IsValid && a.Name == dto.Name) != null;
    }


    public class SubscriptionService(
        AppStorage storage,
        IEnumerable<ISubscriptionUploadStrategy> uploadStrategies,
        IEnumerable<ISubscriptionUpdateStrategy> updateStrategies,
        SubscriptionMapper subscriptionMapper
        )
    {
        public async Task<SubscriptionEntity?> TryReadAsync(Guid? id)
            => await storage.Subscription.AsNoTracking().FirstOrDefaultAsync(s => s.SID == id);
        public async Task<IEnumerable<SubscriptionEntity>> TryReadListAsync()
            => await storage.Account.FirstOrDefaultAsync(e => e.IsValid) is AccountEntity account
                ? await storage.Subscription.AsNoTracking().Where(s => s.UID == account.UID).ToListAsync() : [];

        private void UploadCheck(IEnumerable<SubscriptionDto> dtos)
        {
            if (!storage.Account.Any(e => e.IsValid))
                throw new OperateTransactionFailedException("没有有效账户");

            foreach (var strategy in uploadStrategies)
            {
                var result = strategy.Check(dtos);
                if (!result.IsValid)
                    throw new OperateTransactionFailedException(result.Message);
            }
        }

        private void UpdateCheck(IEnumerable<SubscriptionDto> dtos)
        {
            foreach (var check in updateStrategies)
            {
                var result = check.Check(dtos);
                if (!result.IsValid) throw new OperateTransactionFailedException(result.Message);
            }

        }
        public async Task<IEnumerable<SubscriptionEntity>> TryCreateAsync(params SubscriptionDto[] dtos)
        {
            var transaction = await storage.Database.BeginTransactionAsync();

            try
            {
                UploadCheck(dtos);

                IEnumerable<SubscriptionEntity> entites = dtos.Select(e => subscriptionMapper.ToEntity(e));

                foreach (var entity in entites)
                {
                    entity.Account = await storage.Account.AsNoTracking().FirstOrDefaultAsync(a => a.IsValid) ?? throw new OperateTransactionFailedException("没有有效账户");
                    storage.Subscription.Add(entity);
                }
                await storage.SaveChangesAsync();
                await transaction.CommitAsync();
                return entites;
            }
            catch
            {
                await transaction.RollbackAsync(); throw;
            }
        }


        /// <summary>
        /// 自动识别新增、删除和更新的 Subscription 实体，并进行相应的数据库操作。这个方法假设传入的 dtos 中包含了所有当前有效的订阅信息，任何在数据库中存在但不在 dtos 中的订阅都会被删除，而 dtos 中的新订阅会被添加，已存在的订阅则会被更新。
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ICollection<SubscriptionEntity>>> TryUpdateAsync(Guid guid, IEnumerable<SubscriptionDto> dtos)
        {
            var isNestedTransaction = storage.Database.CurrentTransaction != null;
            var transaction = isNestedTransaction ? null : await storage.Database.BeginTransactionAsync();
            try
            {
                var account = await storage.Account
                    .Include(e => e.Subscriptions)
                    .FirstOrDefaultAsync(e => e.UID == guid)
                    ?? throw new OperateTransactionFailedException("账户不存在");

                var existingLinks = account.Subscriptions.ToDictionary(s => s.SubscriptionLink); // 用字典，查找 O(1)

                var incomingLinks = dtos.Select(s => s.SubscriptionLink).ToHashSet();

                // 删除：数据库有，incoming 没有
                existingLinks.Values
                    .ExceptBy(incomingLinks, s => s.SubscriptionLink)
                    .ToList()
                    .ForEach(s => storage.Subscription.Remove(s));

                // 预分类，避免边遍历边修改集合导致的识别错误
                var toAdd = dtos.Where(d => !existingLinks.ContainsKey(d.SubscriptionLink)).ToList();
                var toUpdate = dtos.Where(d => existingLinks.ContainsKey(d.SubscriptionLink)).ToList();

                // 新增
                if (toAdd.Count > 0)
                {
                    UploadCheck(toAdd);
                    toAdd.ForEach(d => account.Subscriptions.Add(subscriptionMapper.ToEntity(d)));
                }

                // 更新 :  需要添加更新前置安全策略
                if (toUpdate.Count > 0)
                {
                    // 此处check需要检查的是用于更新的内容,而非更新前的原本
                    UpdateCheck(toUpdate);
                    toUpdate.ForEach(d => subscriptionMapper.UpdateEntity(d, existingLinks[d.SubscriptionLink]));
                    //foreach (var d in toUpdate)
                    //{
                    //    subscriptionMapper.UpdateEntity(d, existingLinks[d.SubscriptionLink]);
                    //}
                }
                await storage.SaveChangesAsync();
                if (transaction is not null)
                {
                    await transaction.CommitAsync();
                }
                return storage.Account.Select(e => e.Subscriptions).AsEnumerable();
            }
            catch
            {
                if (transaction is not null) await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> TryDeleteAsync(Guid id)
        {
            SubscriptionEntity entity = await TryReadAsync(id) ?? throw new OperateTransactionFailedException("订阅不存在");
            storage.Subscription.Remove(entity);
            return await storage.SaveChangesAsync();
        }
        public async Task<int> TryAddListAsync(params SubscriptionDto[] subscriptionDtos)
        {
            if (subscriptionDtos == null || subscriptionDtos.Length == 0)
                return 0;
            var list = subscriptionDtos.Select(e => subscriptionMapper.ToEntity(e)).ToList();
            var existingAccount = await storage.Account.Where(e => e.IsValid == true).FirstOrDefaultAsync();
            if (existingAccount != null)
                list.ForEach(e => { e.UID = existingAccount.UID; e.Account = existingAccount; });
            storage.Subscription.AddRange(list);
            return await storage.SaveChangesAsync();
        }
    }
}
