using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Command;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;

namespace MyLife.Service.Validators.BusinessValidator
{
    /// <summary>
    /// 创建时账户名唯一性检查:不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class AccountNameValidator(AppStorage storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "账户名称已存在";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command,CancellationToken ct)
        => !storage.Account.AsNoTracking().Any(a => a.Name == command.Dto.Name);
    }

    /// <summary>
    /// 创建时账户订阅链检查: 至少存在一个订阅
    /// </summary>
    public class SubscriptionCountValidator : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "至少需要一个订阅";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => command.Dto.Subscriptions is not null && command.Dto.Subscriptions.Any();
    }
    /// <summary>
    /// 创建时账户头像以及订阅链接图像检查:不可为空
    /// </summary>
    public class SubscriptionAvatarValidator : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "配置账户的图像文件缺失";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => command.Dto.Avatar is not null && command.Dto.Avatar.Length > 0 && command.Dto.Subscriptions is not null && command.Dto.Subscriptions.Count(e=>e.NewIconFile!=null)==command.Dto.Subscriptions.Count();
    }
    /// <summary>
    /// 创建时订阅平台名称重复性检查: 不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class SubscriptionContentValidator(AppStorage storage):AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "不允许重复的订阅账户平台名称";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        {
            var incomingNames = command.Dto.Subscriptions
            .Select(e => e.SubscriptionPlatform?.Trim())
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

            if (!incomingNames.Any()) return true; // 如果没传订阅，直接放行

            // 2. 检查前端传来的数据自身是否有重复
            // 运用咱们之前学的 HashSet 快速去重对比
            var uniqueIncomingCount = incomingNames.Distinct().Count();
            if (uniqueIncomingCount != incomingNames.Count)
            {
                // 自身数据就有重复，直接拦截
                return false;
            }

            // 3. 检查是否与数据库中已有的名称冲突
            // 优化：利用 Contains(e) 让数据库过滤，只查这几个名字在不在，绝不全表加载！
            bool hasConflictWithDb = storage.Subscription.AsNoTracking()
                .Any(dbSub => incomingNames.Contains(dbSub.AliasName));

            if (hasConflictWithDb)
            {
                // 与数据库已有数据冲突，拦截
                return false;
            }

            return true; // 既无自身重复，也无数据库冲突，放行
        }
    }
    /// <summary>
    /// 登录检查
    /// </summary>
    /// <param name="options"></param>
    /// <param name="storage"></param>
    public class LoginValidator(IOptions<JwtOption> options,AppStorage storage) :AbstractValidator<LoginCommand>
    {
        private protected override string ErrorMessage { init; get; } = "登录失败";
        private protected override async Task<bool> IsValidAsync(LoginCommand command, CancellationToken ct)
            =>command.SecretKey == options.Value.SecretKey && await storage.Account.AsNoTracking().FirstOrDefaultAsync(a => a.IsValid && a.Name == command.AccountName) != null;    
    }

    /// <summary>
    /// 更新时账户头像有效性检查: 头像ID必须有效
    /// </summary>
    /// <param name="storage"></param>
    [Obsolete("重复逻辑,通过DTO注解替代")]
    public class ProfilePictureValidator : AbstractValidator<UpdateAccountAvatarCommand>
    {
        private protected override string ErrorMessage { init; get; } = "头像不存在";
        private protected override async Task<bool> IsValidAsync(UpdateAccountAvatarCommand command, CancellationToken ct)
        => command.Avatar != null;
    }


    [Obsolete("废弃的业务逻辑,不再使用")]
    public class AccountSubscriptionAvatarValidCheckValidator(AppStorage storage) 
    {
        /// <summary>
        /// 实现检查账户的订阅中是否存在无效头像的策略,只处理有效GUID值,如果为null,则skip
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ServiceStatus Check(AccountEntity account)
        {
            var icons = account.Subscriptions.Select(s => s.SubscriptionIcon).Distinct().ToList();// subiicon 存在null 的情况
            if (icons == null || icons.Count == 0)// 这一步基本无法触发, 因为前面已经有订阅检查策略, 但为了保险起见, 还是加上这个判断, 如果没有订阅图标, 则直接返回成功状态
                return new ServiceStatus(string.Empty);
            // 业务场景存在 icon.SubscriptionIcon == null 的情况,所以需要过滤掉 null 的情况,只处理有效的 GUID 值


            if (account.Avatar is Guid avatar) icons.Add(avatar);// 添加 用于更新的ID,在后续检查中,如果存在这个ID,则允许更新,否则不允许

            // 检查icons 这种头像ID是否存在存在, 但因为subIcon 存在 null 情况,所以匹配数量远小于icons.count ; 待处理
            var existCount = storage.File.AsNoTracking().ToList();

            foreach (var icon in icons)
                if (icon is null) continue;
                else if (icon is Guid guid)
                {
                    var isExist = storage.File.AsNoTracking().Any(e => e.UID == guid);
                    if (!isExist)
                    {
                        return new ServiceStatus("订阅头像不存在", false);
                    }
                    else continue;
                }
                else return new ServiceStatus("订阅头像ID无效", false);
            return new ServiceStatus(string.Empty);
        }
    }
}
