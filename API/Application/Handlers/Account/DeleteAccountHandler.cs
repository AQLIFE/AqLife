using MediatR;
using Microsoft.AspNetCore.Http;
using MyLife.Data.Repository;
using MyLife.Domain.Command;
using MyLife.Service;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Account;

public class DeleteAccountHandler(AccountService service) : IRequestHandler<DeleteAccountCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
    {
        await service.TryDeleteAsync(command.UID, ct);
        return Unit.Value;
    }
}
