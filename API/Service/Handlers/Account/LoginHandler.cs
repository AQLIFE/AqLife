using MediatR;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.Account
{
    public class LoginHandler(IJwtProvider<AccountEntity> provider,AccountService service) :IRequestHandler<LoginCommand,string>
    {
        public async Task<string> Handle(LoginCommand command, CancellationToken ct)
        => provider.CreateToken(await service.TryReadAsync(null));
        // 经过业务检查,基本不可能存在空
    }
}
