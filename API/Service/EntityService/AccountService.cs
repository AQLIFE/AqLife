using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Mappings;
using MyLife.Shared.Contracts;
using MyLife.Shared.DTOs;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Tools;

namespace MyLife.Service.EntityService
{
    public class AccountService(
        AccountMapper mapper,
        SubscriptionMapper subscriptionMapper,
        AppStorage storage,
        FileService fileService
        )
    {
        [Obsolete("暂不允许提供到Controller")]
        public async Task<IEnumerable<AccountEntity>> TryReadListAsync()
            => await storage.Account.Include(e => e.Subscriptions).AsNoTracking().Where(a => a.IsValid).ToListAsync();

        public async Task<AccountEntity?> TryReadAsync(Guid? id=null)
            => await storage.Account.Include(e => e.Subscriptions).FirstOrDefaultAsync(a => id == null ? a.IsValid : a.UID == id);

        public async Task<Guid> TryCreateAccountAsync(string name, string? desc, CancellationToken ct)
        {
            AccountEntity entity = new AccountEntity() { Name = name, Desc = desc };
            storage.Account.Add(entity);
            return entity.UID;
        }
        public async Task<Guid> TryCreateAccountAsync(ISimpleAccountInfo dto, IEnumerable<IFormFile> files, CancellationToken ct)
        {
            var entity = mapper.ToEntity(dto);

            // 2. 处理关联文件流（利用注入的 fileService） [cite: 197]
            var fileMetas = await fileService.TryCreateAsync(files, ct);

            // 3. 核心业务规则：分配头像和订阅图标 [cite: 188]
            if (fileMetas.Any())
            {
                entity.Avatar = fileMetas.First();
                var subscriptionIcons = fileMetas.Skip(1).ToList();
                var subscriptions = entity.Subscriptions.ToList();

                // 确保数量匹配时进行赋值
                for (int i = 0; i < Math.Min(subscriptions.Count, subscriptionIcons.Count); i++)
                {
                    subscriptions[i].SubscriptionIcon = subscriptionIcons[i];
                }
            }

            // 4. 持久化
            await storage.Account.AddAsync(entity, ct);
            //await storage.SaveChangesAsync(ct);

            return entity.UID;
        }

        /// <summary>
        /// 负责更新基础信息
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Guid> TryUpdateAsync(Guid guid, string name, string? desc, CancellationToken ct)
        {
            var entity = await TryReadAsync(guid);
            if (entity is AccountEntity account)
            {
                account.Name = name;
                account.Desc = desc;
                return guid;
            }
            return Guid.Empty;
        }
        /// <summary>
        /// 负责更新用户头像
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="avatar"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<Guid> TryUpdateAsync(Guid guid, IFormFile avatar, CancellationToken ct)
        {
            var account = await TryReadAsync(guid);
            if (account is AccountEntity entity)
            {
                var aid = await fileService.TryCreateAsync([avatar], ct);
                if (entity.Avatar is Guid id && id != Guid.Empty) await fileService.TryDeleteAsync([id], ct);// 先删除旧有头像,避免无效文件留存
                entity.Avatar = aid.FirstOrDefault();//再替换ID
                return guid;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 负责更新用户订阅列表
        /// </summary>
        /// <param name="guid"></param>
        /// <param name=""></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// 
        [Obsolete]
        public async Task<Guid> TryUpdateAsync(Guid guid, IEnumerable<SubscriptionFullDto> dtos, CancellationToken ct)
        {
            var account = await TryReadAsync(guid);
            if (account is AccountEntity entity)
            {
                var fileGids = await fileService.TryCreateAsync(dtos.GetIEnumerableFiles(), ct);
                if (fileGids is IEnumerable<Guid?> Gids) await fileService.TryDeleteAsync(Gids, ct);// 先删除旧有头像,避免无效文件留存

                return guid;
            }
            return Guid.Empty;
        }

        public async Task<Guid> TryUpdateAsync(Guid guid, IEnumerable<SubscriptionDto> dtos, CancellationToken ct)
        {
            var account = await TryReadAsync(guid);
            if (account is AccountEntity entity)
            {
                entity.Subscriptions.Clear();
                var subscriptions = dtos.Select(d => subscriptionMapper.ToEntity(d)).ToList();
                subscriptions.ForEach(e => { e.AID = entity.UID;e.Account = entity; });
                await storage.Subscription.AddRangeAsync(subscriptions);
                //entity.Subscriptions = subscriptions;
                return guid;
            }
            throw new RequestTransactionFailedException("账户不存在");
        }

        public async Task TryDeleteAsync(Guid guid, CancellationToken ct)
        {
            //int removedCount = 0;
            var account = await TryReadAsync(guid) ?? throw new RequestTransactionFailedException("账户不存在");

            if (account.Avatar is Guid avatar)
                await fileService.TryDeleteAsync([avatar], ct);// 删除头像
            var subs = account.Subscriptions.Select(s => s.SubscriptionIcon).ToList();

            await fileService.TryDeleteAsync(subs.AsEnumerable(), ct);


            // 删除订阅
            storage.Account.Remove(account);// 按照设计 删除账户会级联删除订阅，因为 SubscriptionEntity 中的 Account 导航属性被配置为 Cascade Delete。
            //return (removedCount += await storage.SaveChangesAsync());
        }
    }


    //public class SubscriptionService(
    //    AppStorage storage,
    //    SubscriptionMapper subscriptionMapper
    //    )
    //{
    //    public async Task<SubscriptionEntity?> TryReadAsync(Guid? id)
    //        => await storage.Subscription.AsNoTracking().FirstOrDefaultAsync(s => s.SID == id);
    //    public async Task<IEnumerable<SubscriptionEntity>> TryReadListAsync()
    //        => await storage.Account.FirstOrDefaultAsync(e => e.IsValid) is AccountEntity account
    //            ? await storage.Subscription.AsNoTracking().Where(s => s.UID == account.UID).ToListAsync() : [];

    //    //private void UploadCheck(IEnumerable<SubscriptionDto> dtos)
    //    //{
    //    //    if (!storage.Account.Any(e => e.IsValid))
    //    //        throw new OperateTransactionFailedException("没有有效账户");

    //    //    foreach (var strategy in uploadStrategies)
    //    //    {
    //    //        var result = strategy.Check(dtos);
    //    //        if (!result.IsValid)
    //    //            throw new OperateTransactionFailedException(result.Message);
    //    //    }
    //    //}

    //    //private void UpdateCheck(IEnumerable<SubscriptionDto> dtos)
    //    //{
    //    //    foreach (var check in updateStrategies)
    //    //    {
    //    //        var result = check.Check(dtos);
    //    //        if (!result.IsValid) throw new OperateTransactionFailedException(result.Message);
    //    //    }

    //    //}
    //    //public async Task<IEnumerable<SubscriptionEntity>> TryCreateAsync(params SubscriptionDto[] dtos)
    //    //{
    //    //    var transaction = await storage.Database.BeginTransactionAsync();

    //    //    try
    //    //    {
    //    //        UploadCheck(dtos);

    //    //        IEnumerable<SubscriptionEntity> entites = dtos.Select(e => subscriptionMapper.ToEntity(e));

    //    //        foreach (var entity in entites)
    //    //        {
    //    //            entity.Account = await storage.Account.FirstOrDefaultAsync(a => a.IsValid) ?? throw new OperateTransactionFailedException("没有有效账户");
    //    //            storage.Subscription.Add(entity);
    //    //        }
    //    //        await storage.SaveChangesAsync();
    //    //        await transaction.CommitAsync();
    //    //        return entites;
    //    //    }
    //    //    catch
    //    //    {
    //    //        await transaction.RollbackAsync(); throw;
    //    //    }
    //    //}


    //    /// <summary>
    //    /// 自动识别新增、删除和更新的 Subscription 实体，并进行相应的数据库操作。这个方法假设传入的 dtos 中包含了所有当前有效的订阅信息，任何在数据库中存在但不在 dtos 中的订阅都会被删除，而 dtos 中的新订阅会被添加，已存在的订阅则会被更新。
    //    /// </summary>
    //    /// <param name="guid"></param>
    //    /// <param name="dtos"></param>
    //    /// <returns></returns>
    //    //public async Task<IEnumerable<ICollection<SubscriptionEntity>>> TryUpdateAsync(AccountEntity account, IEnumerable<SubscriptionDto> dtos)
    //    //{
    //    //    var isNestedTransaction = storage.Database.CurrentTransaction != null;
    //    //    var transaction = isNestedTransaction ? null : await storage.Database.BeginTransactionAsync();
    //    //    try
    //    //    {
    //    //        //var account = await storage.Account
    //    //        //    .Include(e => e.Subscriptions)
    //    //        //    .FirstOrDefaultAsync(e => e.UID == guid)
    //    //        //    ?? throw new OperateTransactionFailedException("账户不存在");

    //    //        var existingLinks = account.Subscriptions.ToDictionary(s => s.SubscriptionLink); // 用字典，查找 O(1)

    //    //        var incomingLinks = dtos.Select(s => s.SubscriptionLink).ToHashSet();

    //    //        // 删除：数据库有，incoming 没有
    //    //        existingLinks.Values
    //    //            .ExceptBy(incomingLinks, s => s.SubscriptionLink)
    //    //            .ToList()
    //    //            .ForEach(s => storage.Subscription.Remove(s));

    //    //        // 预分类，避免边遍历边修改集合导致的识别错误
    //    //        var toAdd = dtos.Where(d => !existingLinks.ContainsKey(d.SubscriptionLink)).ToList();
    //    //        var toUpdate = dtos.Where(d => existingLinks.ContainsKey(d.SubscriptionLink)).ToList();

    //    //        // 新增
    //    //        // csharp
    //    //        if (toAdd.Count > 0)
    //    //        {
    //    //            UploadCheck(toAdd);
    //    //            foreach (var d in toAdd)
    //    //            {
    //    //                var entity = subscriptionMapper.ToEntity(d);
    //    //                // 显式绑定到父实体，避免因映射/注解问题导致关系丢失
    //    //                entity.UID = account.UID;
    //    //                entity.Account = account;
    //    //                storage.Subscription.Add(entity);
    //    //                //storage.Subscription.Entry(entity).State = EntityState.Added;
    //    //            }
    //    //        }

    //    //        // 更新 :  需要添加更新前置安全策略
    //    //        if (toUpdate.Count > 0)
    //    //        {
    //    //            // 此处check需要检查的是用于更新的内容,而非更新前的原本
    //    //            UpdateCheck(toUpdate);
    //    //            toUpdate.ForEach(d => subscriptionMapper.UpdateEntity(d, existingLinks[d.SubscriptionLink]));
    //    //        }
    //    //        var entries = storage.ChangeTracker.Entries()
    //    //            .Select(e => new
    //    //            {
    //    //                Type = e.Entity.GetType().FullName,
    //    //                State = e.State.ToString(),
    //    //                Keys = e.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue).ToList(),
    //    //                Original = e.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue),
    //    //                Current = e.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)
    //    //            }).ToList();
    //    //        // 把 entries 输出到日志或调试窗口
    //    //        await storage.SaveChangesAsync();// csharp
    //    //        if (transaction is not null)
    //    //        {
    //    //            await transaction.CommitAsync();
    //    //        }
    //    //        return await storage.Account.Select(e => e.Subscriptions).ToListAsync();
    //    //    }
    //    //    catch
    //    //    {
    //    //        if (transaction is not null) await transaction.RollbackAsync();
    //    //        throw;
    //    //    }
    //    //}

    //    //public async Task<int> TryDeleteAsync(Guid id)
    //    //{
    //    //    SubscriptionEntity entity = await TryReadAsync(id) ?? throw new OperateTransactionFailedException("订阅不存在");
    //    //    storage.Subscription.Remove(entity);
    //    //    return await storage.SaveChangesAsync();
    //    //}
    //    //public async Task<int> TryAddListAsync(params SubscriptionDto[] subscriptionDtos)
    //    //{
    //    //    if (subscriptionDtos == null || subscriptionDtos.Length == 0)
    //    //        return 0;
    //    //    var list = subscriptionDtos.Select(e => subscriptionMapper.ToEntity(e)).ToList();
    //    //    var existingAccount = await storage.Account.Where(e => e.IsValid == true).FirstOrDefaultAsync();
    //    //    if (existingAccount != null)
    //    //        list.ForEach(e => { e.UID = existingAccount.UID; e.Account = existingAccount; });
    //    //    storage.Subscription.AddRange(list);
    //    //    return await storage.SaveChangesAsync();
    //    //}
    //}
}
