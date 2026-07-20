using MediatR;
using MyLife.Domain.Entities;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.Account
{
    public class GetAccountHandler(AccountService service, AccountMapper mapper) : IRequestHandler<AccountQuery, AccountDto?>
    {
        public async Task<AccountDto?> Handle(AccountQuery query, CancellationToken ct)
        {
            var result = await service.TryReadAsync(null);
            return result is AccountEntity account ? mapper.ToDto(account) : null;// 存在业务检查,所以基本不会出现无效ID
        }
    }
}
