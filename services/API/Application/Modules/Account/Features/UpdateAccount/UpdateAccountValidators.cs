using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Abstractions.Persistence.Queries;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Utils;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Validator
{
    public class AvatarExtensionValidator : AbstractValidator<UpdateAccountAvatarCommand>
    {
        readonly HashSet<string> Extension = [".png", ".jpeg", ".jpg"];
        protected override string ErrorMessage { init; get; } = "更新账户头像必须是图像类型";
        protected override async Task<bool> IsValidAsync(UpdateAccountAvatarCommand command, CancellationToken ct)
            => Extension.Contains(Path.GetExtension(command.Avatar.FileName).ToLowerInvariant());
    }

    public class SubscriptionIconExistenceValidator(IFileMetaQuerySource fileQuery) : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        protected override string ErrorMessage { init; get; } = "配置账户的图像文件不存在";
        protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            var iconsToCheck = command.Subscriptions.Where(s => s.SubscriptionIcon != Guid.Empty)
                .Select(e => e.SubscriptionIcon).Distinct().ToList();
            var existingCount = await fileQuery.Query()
                .Where(f => iconsToCheck.Contains(f.UID)).CountAsync(ct);
            return existingCount == iconsToCheck.Count;
        }
    }

    public class UpdateAccount_InvalidName_ShouldRejectValidator : AbstractValidator<UpdateAccountProfileCommand>
    {
        protected override string ErrorMessage { init; get; } = "账户名称不允许存在空格";
        protected override async Task<bool> IsValidAsync(UpdateAccountProfileCommand command, CancellationToken ct)
            => !string.IsNullOrWhiteSpace(command.Name) && command.Name.All(c => !char.IsWhiteSpace(c));
    }

    public class SubscriptionIconExtensionValidator(IFileMetaQuerySource fileQuery) : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        protected override string ErrorMessage { init; get; } = "订阅图像必须是SVG文件类型";
        protected override async Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            IEnumerable<Guid> guids = command.Subscriptions.Select(e => e.SubscriptionIcon);
            List<FileMetaEntity> files = await fileQuery.Query().AsNoTracking()
                .Where(r => guids.Contains(r.UID)).ToListAsync(ct);
            return files.All(e => e.Extension == ".svg");
        }
    }

    public class SubscriptionContentValidator : AbstractValidator<UpdateAccountSubscriptionsCommand>
    {
        protected override string ErrorMessage { init; get; } = "不允许重复的订阅账户平台名称";
        protected override Task<bool> IsValidAsync(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            var names = command.Subscriptions.Select(e => e.SubscriptionPlatform?.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
            return Task.FromResult(names.Count == names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        }
    }

    public class UpdatePasswordValidator(IAccountPersistence persistence) : AbstractValidator<UpdatePasswordCommand>
    {
        protected override string ErrorMessage { init; get; } = "错误的密码";
        protected override async Task<bool> IsValidAsync(UpdatePasswordCommand command, CancellationToken ct)
        {
            AccountEntity account = await persistence.Accounts.AsNoTracking()
                .SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("账户不存在");
            return account.LoginPasswordHash == FastHash.GetSha256Hash(command.OldPassword);
        }
    }
}
