using MediatR;
using Microsoft.EntityFrameworkCore;
using MyLife.Application.Business.File.Service;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.Exceptions;

namespace MyLife.Application.Business.Account.Handler;

public class DeleteAccountHandler(AppStorage storage, FileDeleter fileDeleter) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
    {
        AccountEntity account = await storage.Account.Include(e => e.Subscriptions).SingleOrDefaultAsync(e => e.UID == command.UID && e.IsValid) ?? throw new ResourceNotFoundException("账户不存在");

        var subs = account.Subscriptions.Select(s => s.SubscriptionIcon).ToList();
        if (account.Avatar is Guid avatar) subs.Add(avatar);
        await fileDeleter.DeleteAsync(subs.AsEnumerable(), ct);
        storage.Account.Remove(account);
        return Unit.Value;
    }
}
