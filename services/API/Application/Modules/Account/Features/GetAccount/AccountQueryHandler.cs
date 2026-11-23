using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Account.Handler
{
    // 完成 Query 和 Service 业务类的解耦,使用Search业务类完成 Query 的处理,并使用 Mapper 将实体转换为 DTO;进度1
    public class AccountQueryHandler(ISearch<AccountQuery, AccountDto, AccountEntity> search, IViewMapper<AccountEntity, AccountDto> mapper) : IRequestHandler<AccountQuery, AccountDto?>
    {
        public async Task<AccountDto?> Handle(AccountQuery query, CancellationToken ct)
        {
            var result = await search.SearchAsync(query, ct);
            var account = result.SingleOrDefault(e => e.IsValid);
            return account is null ? null : mapper.ToDto(account);// 存在业务检查,所以基本不会出现无效ID
        }
    }
}
