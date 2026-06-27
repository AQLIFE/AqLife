using MediatR;
using MyLife.Application.Command;
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
