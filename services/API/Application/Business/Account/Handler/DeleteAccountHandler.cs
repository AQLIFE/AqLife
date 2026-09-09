using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Account.Handler;

public class DeleteAccountHandler(IApplicationDbContext storage, FileDeleter fileDeleter) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
    {
        AccountEntity account = await storage.Accounts.Include(e => e.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID) ?? throw new ResourceNotFoundException("账户不存在");

        var subs = account.Subscriptions.Select(s => s.SubscriptionIcon).ToList();
        if (account.Avatar is Guid avatar) subs.Add(avatar);
        await fileDeleter.DeleteAsync(subs.AsEnumerable(), ct);
        storage.Accounts.Remove(account);
        return Unit.Value;
    }
}
