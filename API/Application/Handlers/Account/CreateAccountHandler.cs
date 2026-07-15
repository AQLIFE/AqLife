using MediatR;
using MyLife.Application.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Account
{
    public class CreateAccountHandler(AccountService service) : IRequestHandler<CreateAccountCommand, string>
    {
        public async Task<string> Handle(CreateAccountCommand request, CancellationToken ct)
       => await service.TryCreateAccountAsync(request.Name, request.Desc,request.pwd, ct);
    }
}
