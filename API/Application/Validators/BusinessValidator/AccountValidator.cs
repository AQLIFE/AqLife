using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Application.Command;
using MyLife.Data.Repository;
using MyLife.Shared.Options;

namespace MyLife.Application.Validators.BusinessValidator
{
    /// <summary>
    /// 创建时账户名唯一性检查:不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class AccountNameValidator(AppStorage storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "账户名称已存在";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !storage.Account.AsNoTracking().Any(a => a.Name == command.Name);
    }

    /// <summary>
    /// 创建时账户唯一性检查: 不允许创建第二个账户
    /// </summary>
    /// <param name="storage"></param>
    public class AccountUniqueValidator(AppStorage storage) : AbstractValidator<CreateAccountCommand>
    {
        private protected override string ErrorMessage { init; get; } = "仅允许注册一个账户";
        private protected override async Task<bool> IsValidAsync(CreateAccountCommand command, CancellationToken ct)
        => !await storage.Account.AsNoTracking().AnyAsync();
    }



    /// <summary>
    /// 更新时账户头像以及订阅链接图像检查:不可为空
    /// </summary>
    public class SubscriptionImageValidator : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "配置账户的图像文件缺失";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        => command.Subscriptions is not null
            && command.Subscriptions.Count() > 0
            && command.Subscriptions.Count(e => e.NewIconFile != null) == command.Subscriptions.Count();
    }
    /// <summary>
    /// 更新时订阅平台名称重复性检查: 不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class SubscriptionContentValidator : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "不允许重复的订阅账户平台名称";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            var incomingNames = command.Subscriptions
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
            return true; // 既无自身重复，也无数据库冲突，放行
        }
    }
    /// <summary>
    /// 登录检查
    /// </summary>
    /// <param name="options"></param>
    /// <param name="storage"></param>
    public class LoginValidator(IOptions<JwtOption> options, AppStorage storage) : AbstractValidator<LoginCommand>
    {
        private protected override string ErrorMessage { init; get; } = "登录失败";
        private protected override async Task<bool> IsValidAsync(LoginCommand command, CancellationToken ct)
            => command.SecretKey == options.Value.SecretKey && await storage.Account.AsNoTracking().FirstOrDefaultAsync(a => a.IsValid && a.Name == command.AccountName) != null;
    }
}
