using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Shared.Options;
using MyLife.Shared.Utils;

namespace MyLife.Application.Business.Account.Validator
{
    public class AvatarExtensionValidator : AbstractValidator<UpdateAccountAvatarCommand>
    {
        private readonly HashSet<string> Extension = [".png", ".jpeg", ".jpg"];
        private protected override string ErrorMessage { init; get; } = "更新账户头像必须是图像类型";
        private protected override async Task<bool> IsValidAsync(UpdateAccountAvatarCommand command, CancellationToken ct)
        => Extension.Contains(Path.GetExtension(command.Avatar.FileName).ToLowerInvariant());
    }

    /// <summary>
    /// 更新时账户头像以及订阅链接图像检查:不可为空
    /// </summary>
    public class SubscriptionIconRequiredValidator : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "配置账户的图像文件缺失";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        => command.Subscriptions is not null
            && command.Subscriptions.Count() > 0
            && command.Subscriptions.Count(e => e.SubscriptionIcon != null) == command.Subscriptions.Count();
    }

    /// <summary>
    ///  更新的订阅ID 必须全部为有效ID
    /// </summary>
    /// <param name="storage"></param>
    public class SubscriptionIconExistenceValidator(IApplicationDbContext storage) : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "配置账户的图像文件信息不存在";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            // 1. 提取所有非空且非 Empty 的 GUID（去重），实现“按需验证” [cite: 16]
            var iconsToCheck = command.Subscriptions
                .Where(s => s.SubscriptionIcon.HasValue && s.SubscriptionIcon.Value != Guid.Empty && s.SubscriptionIcon is Guid)
                .Select(s => s.SubscriptionIcon!.Value)
                .Distinct()
                .ToList();

            // 3. 数据库侧验证：仅查询存在的数量是否与待检查数量一致 [cite: 27, 28]
            var existingCount = await storage.File
                .Where(f => iconsToCheck.Contains(f.UID))
                .CountAsync(ct);

            return existingCount == iconsToCheck.Count;
        }
    }

    /// <summary>
    /// 更新时订阅平台名称重复性检查: 不允许重复
    /// </summary>
    /// <param name="storage"></param>
    public class SubscriptionContentValidator : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "不允许重复的订阅账户平台名称";
        private protected override Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            var names = command.Subscriptions
                .Select(e => e.SubscriptionPlatform?.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .ToList();

            return Task.FromResult(
                names.Count == names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        }
    }

    public class UpdatePasswordValidator(IApplicationDbContext dbContext):AbstractValidator<UpdatePasswordCommand>
    {
        private protected override string ErrorMessage { init; get; } = "错误的密码";
        private protected override async Task<bool> IsValidAsync(UpdatePasswordCommand command, CancellationToken ct)
        {
            AccountEntity account = await dbContext.Accounts.AsNoTracking().SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            return account.LoginPasswordHash == FastHash.GetSha256Hash(command.OldPassword);
        }
    }
}
