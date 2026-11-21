using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Account.Handler
{
    public class UpdateAccountProfileHandler(IApplicationDbContext storage) : IRequestHandler<UpdateAccountProfileCommand, string>
    {
        public async Task<string> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.UpdateProfile(command.Name, command.Desc ?? string.Empty);
            return account.LoginName;
        }
    }


    // 需要准备用 Notification 优化业务表达 ,纳入 plan 2
    public class UpdateAccountAvatarHandler(IApplicationDbContext storage, IFileAssetService fileAssetService) : IRequestHandler<UpdateAccountAvatarCommand, string>
    {
        public async Task<string> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            Guid newId = await fileAssetService.UploadAsync(command.Avatar, CategoryEntity.DefaultCategoryId, ct);
            Guid oldId = account.Avatar;

            await fileAssetService.PublishAsync(newId, ct);
            account.ChangeAvatar(newId);

            await fileAssetService.DeleteAsync([oldId], ct);
            return account.LoginName;
        }
    }

    /// <summary>
    /// 处理订阅列表更新，并自动发布其中的图像
    /// </summary>
    public class UpdateAccountSubscriptionsHandler(
        IApplicationDbContext storage,
        ICreateMapper<SubscriptionEntity, ISubscription> subscriptionMapper,
        IFileAssetService fileAssetService)
        : IRequestHandler<UpdateAccountSubscriptionsCommand, string>
    {
        public async Task<string> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            // 前置验证管道负责检查所有文件标识是否有效。
            AccountEntity account = await storage.Accounts
                .Include(a => a.Subscriptions)
                .SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("账户不存在");

            await fileAssetService.PublishAsync(
                command.Subscriptions.Select(e => e.SubscriptionIcon),
                ct);

            // 明确维护 EF Core 实体状态，避免仅依赖导航集合 Clear/Add 的隐式关系变更检测。
            var newSubscriptions = command.Subscriptions
                .Select(subscriptionMapper.ToEntity)
                .ToList();

            foreach (var subscription in newSubscriptions)
                subscription.BindAccount(account);

            storage.Subscription.RemoveRange(account.Subscriptions);
            storage.Subscription.AddRange(newSubscriptions);

            return account.LoginName;
        }
    }

    public class UpdatePasswordHandler(IApplicationDbContext storage) : IRequestHandler<UpdatePasswordCommand, string>
    {
        public async Task<string> Handle(UpdatePasswordCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.ChangePassword(command.NewPassword);
            return account.LoginName;
        }
    }
}
