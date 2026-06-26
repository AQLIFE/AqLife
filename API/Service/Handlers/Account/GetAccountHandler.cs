using MediatR;
using Microsoft.AspNetCore.Http;
using MyLife.Data.Entities;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Shared.DTOs;
using MyLife.Shared.Tools;

namespace MyLife.Service.Handlers.Account
{
    public class GetAccountHandler(AccountService service, AccountMapper mapper) : IRequestHandler<GetAccountQuery, AccountDto?>
    {
        public async Task<AccountDto?> Handle(GetAccountQuery query, CancellationToken ct)
        {
            var result = await service.TryReadAsync(null);
            return result is AccountEntity account? mapper.ToDto(account):null;// 存在业务检查,所以基本不会出现无效ID
        }
    }
}
