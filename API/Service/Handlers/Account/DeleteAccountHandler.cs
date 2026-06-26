using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.Account;

public class DeleteAccountHandler(AccountService service) : IRequestHandler<DeleteAccountCommand,Unit>
{
    public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
    {
        await service.TryDeleteAsync(command.UID, ct);
        return Unit.Value;
    }
}
