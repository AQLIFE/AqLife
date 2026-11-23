using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Handler
{
    public class UpdateAccountProfileHandler(IAccountPersistence persistence) : IRequestHandler<UpdateAccountProfileCommand, string>
    {
        public async Task<string> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        {
            AccountEntity account = await persistence.Accounts.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.UpdateProfile(command.Name, command.Desc ?? string.Empty);
            return account.LoginName;
        }
    }

    // 需要准备用 Notification 优化业务表达 ,纳入 plan 2
    public class UpdateAccountAvatarHandler(IAccountPersistence persistence, IApplicationDbContext dbContext, IFileAssetService fileAssetService) : IRequestHandler<UpdateAccountAvatarCommand, string>
    {
        public async Task<string> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
        {
            AccountEntity account = await persistence.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            Guid newId = await fileAssetService.UploadToDefaultCategoryAsync(command.Avatar, ct);
            Guid oldId = account.Avatar;
            if (newId == oldId)
                return account.LoginName;

            await fileAssetService.PublishAsync(newId, ct);
            account.ChangeAvatar(newId);

            // 先保存新的头像引用，再让 File 模块判断旧文件是否仍被其他模块引用。
            await dbContext.SaveChangesAsync(ct);
            await fileAssetService.DeleteAsync([oldId], ct);
            return account.LoginName;
        }
    }

    public class UpdateAccountSubscriptionsHandler(
        IAccountPersistence persistence,
        ICreateMapper<SubscriptionEntity, ISubscription> subscriptionMapper,
        IFileAssetService fileAssetService)
        : IRequestHandler<UpdateAccountSubscriptionsCommand, string>
    {
        public async Task<string> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            AccountEntity account = await persistence.Accounts
                .Include(a => a.Subscriptions)
                .SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("账户不存在");

            await fileAssetService.PublishAsync(command.Subscriptions.Select(e => e.SubscriptionIcon), ct);

            var newSubscriptions = command.Subscriptions
                .Select(subscriptionMapper.ToEntity)
                .ToList();

            foreach (var subscription in newSubscriptions)
                subscription.BindAccount(account);

            persistence.RemoveSubscriptions(account.Subscriptions);
            persistence.AddSubscriptions(newSubscriptions);
            return account.LoginName;
        }
    }

    public class UpdatePasswordHandler(IAccountPersistence persistence) : IRequestHandler<UpdatePasswordCommand, string>
    {
        public async Task<string> Handle(UpdatePasswordCommand command, CancellationToken ct)
        {
            AccountEntity account = await persistence.Accounts.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.ChangePassword(command.NewPassword);
            return account.LoginName;
        }
    }
}
