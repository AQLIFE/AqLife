using MediatR;
using Microsoft.AspNetCore.Http;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.DTOs;
using MyLife.Shared.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.Handlers.Account
{
    public class GetAccountHandler(AccountService service, IHttpContextAccessor httpContext,AccountMapper mapper) :IRequestHandler<GetAccountQuery,AccountDto>
    {
        public async Task<AccountDto> Handle(GetAccountQuery query,CancellationToken ct)
        {
            var result  = await service.TryReadAsync(httpContext.HttpContext?.User.GetAccountId());
            return mapper.ToDto(result);// 村子啊业务检查,所以基本不会出现无效ID
        }
    }
}
