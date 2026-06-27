using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Account
{
    public class CreateAccountHandler(AccountService service) : IRequestHandler<CreateAccountCommand, Guid>
    {
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken ct)
       => await service.TryCreateAccountAsync(request.Name, request.Desc, ct);
    }
}
