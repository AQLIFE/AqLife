using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Services;

/// <summary>
/// Account 模块拥有的文件引用检查：账户头像和订阅图标。
/// </summary>
public sealed class AccountFileReferenceProvider(IApplicationDbContext dbContext)
    : IFileReferenceProvider
{
    public async Task<bool> IsReferencedAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        var usedByAvatar = await dbContext.Accounts
            .AnyAsync(account => account.IsValid && account.Avatar == fileId, cancellationToken);

        if (usedByAvatar)
            return true;

        return await dbContext.Accounts
            .Where(account => account.IsValid)
            .SelectMany(account => account.Subscriptions)
            .AnyAsync(subscription => subscription.SubscriptionIcon == fileId, cancellationToken);
    }
}
