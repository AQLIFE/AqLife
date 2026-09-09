using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Business.Account;
using AqLife.Shared.Exceptions;

namespace AqLife.Application.Business.Account.Handler
{
    public class UpdateAccountProfileHandler(IApplicationDbContext storage) : IRequestHandler<UpdateAccountProfileCommand, string>
    {
        public async Task<string> Handle(UpdateAccountProfileCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");
            account.UpdateProfile(command.Name, command.Desc);
            return account.LoginName;
        }
    }


    // 需要准备用 Notification 优化业务表达 ,纳入 plan 2
    public class UpdateAccountAvatarHandler(IApplicationDbContext storage, FileWriter fileWriter, FileDeleter fileDeleter) : IRequestHandler<UpdateAccountAvatarCommand, string>
    {
        public async Task<string> Handle(UpdateAccountAvatarCommand command, CancellationToken ct)
        //=> await service.TryUpdateAsync(command.UID, command.Avatar, ct);
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            var aid = await fileWriter.WriteAsync([command.Avatar], ct); // 先创建 对应文件,这样即使后面失败了也没有太大影响
            if (account.Avatar is Guid id && id != Guid.Empty) await fileDeleter.DeleteAsync([id], ct);// 删除旧有头像,避免无效文件留存; 这一步也可以放弃,目前设计哪怕只替换GUID都是可以的
            account.ChangeAvatar(aid.FirstOrDefault());
            return account.LoginName;
        }
    }

    public class UpdateAccountSubscriptionsHandler(IApplicationDbContext storage, SubscriptionMapper subscriptionMapper) : IRequestHandler<UpdateAccountSubscriptionsCommand, string>
    {
        public async Task<string> Handle(UpdateAccountSubscriptionsCommand command, CancellationToken ct)
        {
            AccountEntity account = await storage.Accounts.Include(a => a.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("账户不存在");

            account.ReplaceSubscriptions(command.Subscriptions.Select(subscriptionMapper.ToEntity));
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
