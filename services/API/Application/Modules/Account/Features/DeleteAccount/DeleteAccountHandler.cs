using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Account.Handler;

public class DeleteAccountHandler(
    IApplicationDbContext dbContext,
    IFileAssetService fileAssetService)
    : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
    {
        AccountEntity account = await dbContext.Accounts
            .Include(e => e.Subscriptions)
            .SingleOrDefaultAsync(e => e.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("账户不存在");

        var fileIds = account.Subscriptions
            .Select(subscription => subscription.SubscriptionIcon)
            .ToList();

        if (account.Avatar is Guid avatar)
            fileIds.Add(avatar);

        // 先在当前事务内释放 Account 模块持有的引用，
        // File 模块随后才能准确判断文件是否仍被 Favorite 等模块使用。
        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(ct);

        await fileAssetService.DeleteAsync(fileIds, ct);
        return Unit.Value;
    }
}
