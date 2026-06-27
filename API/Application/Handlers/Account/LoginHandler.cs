using MediatR;
using MyLife.Application.Command;
using MyLife.Data.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Service.EntityService;
using MyLife.Service.ServiceInterfaces;

namespace MyLife.Application.Handlers.Account
{
    public class LoginHandler(IJwtProvider<AccountEntity> provider, AccountService service) : IRequestHandler<LoginCommand, string>
    {
        public async Task<string> Handle(LoginCommand command, CancellationToken ct)
        => provider.CreateToken(await service.TryReadAsync()??throw new RequestTransactionFailedException("授权失败"));
        // 经过业务检查,基本不可能存在空
    }
}
