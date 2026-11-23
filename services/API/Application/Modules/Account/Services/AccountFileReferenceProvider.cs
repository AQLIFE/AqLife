using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence.Modules;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Services;

/// <summary>
/// Account 模块拥有的文件引用检查：所有持久化账户的头像和订阅图标。
/// 账户被禁用不代表其文件引用已释放。
/// </summary>
public sealed class AccountFileReferenceProvider(IAccountPersistence persistence)
    : IFileReferenceProvider
{
    public async Task<bool> IsReferencedAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var usedByAvatar = await persistence.Accounts
            .AnyAsync(account => account.Avatar == fileId, cancellationToken);
        if (usedByAvatar)
            return true;

        return await persistence.Accounts
            .SelectMany(account => account.Subscriptions)
            .AnyAsync(subscription => subscription.SubscriptionIcon == fileId, cancellationToken);
    }
}
