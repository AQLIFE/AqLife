using MediatR;
using MyLife.Shared.Exceptions;
using MyLife.Service.Interfaces;
using MyLife.Domain.Entities;
using MyLife.Domain.Command;
using MyLife.Service.EntityService;

namespace MyLife.Application.Handlers.Account
{
    public class LoginHandler(IJwtProvider<AccountEntity> provider, AccountService service) : IRequestHandler<LoginCommand, string>
    {
        public async Task<string> Handle(LoginCommand command, CancellationToken ct)
        => provider.CreateToken(await service.TryReadAsync()??throw new RequestTransactionFailedException("授权失败"));
        // 经过业务检查,基本不可能存在空
    }
}
