using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Business.Account;
using AqLife.Shared.Exceptions;
using AqLife.Application.BackServices;

namespace AqLife.Application.Business.Account.Handler
{
    public class UpdateAccountProfileHandler(IApplicationDbContext storage) : IRequestHandler<UpdateAccountProfileCommand, string>
    {
        public async Task<string> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.UpdateProfile(command.Name, command.Desc??string.Empty);
            return account.LoginName;
        }
    }


    // 需要准备用 Notification 优化业务表达 ,纳入 plan 2
    public class UpdateAccountAvatarHandler(IApplicationDbContext storage, FileWriter fileWriter, FileDeleter fileDeleter,IFilePublishService filePublishService) : IRequestHandler<UpdateAccountAvatarCommand, string>
    {
        public async Task<string> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            List<Guid> newIds = await fileWriter.WriteAsync([command.Avatar], ct); // 先创建 对应文件,这样即使后面失败了也没有太大影响
            Guid oldId = account.Avatar;

            _ = await filePublishService.PublishAsync(newIds, ct);// 先发布
            account.ChangeAvatar(newIds.FirstOrDefault());      // 再更新

            await fileDeleter.DeleteAsync([oldId], ct);// 删除旧有头像,避免无效文件留存;
            return account.LoginName;
        }
    }

    /// <summary>
    /// 处理 订阅列表 更新,并自动发布其中的图像
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="subscriptionMapper"></param>
    public class UpdateAccountSubscriptionsHandler(IApplicationDbContext storage, SubscriptionMapper subscriptionMapper,IFilePublishService filePublishService) : IRequestHandler<UpdateAccountSubscriptionsCommand, string>
    {
        public async Task<string> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            // 前置验证管道 负责检查 所有guid 是否属于 图像资源
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            _ = await filePublishService.PublishAsync(command.Subscriptions.Select(e => e.SubscriptionIcon), ct);// 该业务链路,默认传递的只有guid,没有file,所以不需要在该链路执行文件上传;所有的guid 一定是有效的guid值
            // 这个方法,就只会发布所有存在且未发布(草稿和预约)的guid,因为在该链路之前,默认执行过文件上传,所以大部分预期都是草稿图像文件,流程合规可控

            // 明确维护 EF Core 实体状态，避免仅依赖导航集合 Clear/Add 的隐式关系变更检测。
            // 旧订阅标记为 Deleted，新订阅标记为 Added；事务管道随后统一 SaveChangesAsync。
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
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.ChangePassword(command.NewPassword);
            return account.LoginName;
        }
    }
}
