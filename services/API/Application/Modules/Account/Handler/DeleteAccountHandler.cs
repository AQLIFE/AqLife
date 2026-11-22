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

        await fileAssetService.DeleteAsync(fileIds, ct);
        dbContext.Accounts.Remove(account);

        return Unit.Value;
    }
}
