using MediatR;
using MyLife.Service.Command;
using MyLife.Service.EntityService;

namespace MyLife.Service.Handlers.Account
{
    public class CreateAccountHandler(AccountService service) : IRequestHandler<CreateAccountCommand, Guid>
    {
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken ct)
       => await service.TryCreateAccountAsync(request, ct);
    }
}
