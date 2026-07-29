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
    /// <summary>
    /// 更新头像必须是图像类型
    /// </summary>
    public class AvatarExtensionValidator : AbstractValidator<UpdateAccountAvatarCommand>
    {
        private readonly HashSet<string> Extension = [".png", ".jpeg", ".jpg"];
        private protected override string ErrorMessage { init; get; } = "更新账户头像必须是图像类型";
        private protected override async Task<bool> IsValidAsync(UpdateAccountAvatarCommand command, CancellationToken ct)
        => Extension.Contains(Path.GetExtension(command.Avatar.FileName).ToLowerInvariant());
    }

    /// <summary>
    ///  更新的订阅ID 必须全部为有效ID
    /// </summary>
    /// <param name="storage"></param>
    public class SubscriptionIconExistenceValidator(IApplicationDbContext storage) : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "配置账户的图像文件不存在";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            // 1. 提取所有非空且非 Empty 的 GUID（去重），实现“按需验证” [cite: 16]
            var iconsToCheck = command.Subscriptions.Where(s => s.SubscriptionIcon != Guid.Empty).Select(e=>e.SubscriptionIcon).Distinct().ToList();

            // 3. 数据库侧验证：仅查询存在的数量是否与待检查数量一致 [cite: 27, 28]
            var existingCount = await storage.File.Where(f => iconsToCheck.Contains(f.UID)).CountAsync(ct);

            return existingCount == iconsToCheck.Count;
        }
    }

    public class SubscriptionIconExtensionValidator(IApplicationDbContext dbContext) : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        private protected override string ErrorMessage { init; get; } = "订阅图像必须是SVG文件类型";
        private protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            IEnumerable<Guid> guids = command.Subscriptions.Select(e => e.SubscriptionIcon).AsEnumerable();
            List<FileMetaEntity> files = await dbContext.File.AsNoTracking().Where(r => guids.Contains(r.UID)).ToListAsync();
            return files.All(e => e.Extension == ".svg");
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
