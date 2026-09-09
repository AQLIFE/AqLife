using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Mappers;
namespace AqLife.Application.Business.Account.Search
{
    // 尝试使用BaseSearch来实现AccountSearch，减少重复代码,等待测试
    public class AccountSearch(QueryMapper queryMapper, IApplicationDbContext storage, IEnumerable<ISearchStrategy<AccountEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<AccountQuery, AccountEntity, AccountDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override async Task<IQueryable<AccountEntity>> BuildBaseQueryAsync(IQueryable<AccountEntity> queryable, AccountQuery query)
        => queryable.Include(e => e.Subscriptions);

        protected override EntitySearchCriteria MapToCriteria(AccountQuery query)
        => queryMapper.ToCriteria(query);
    }
}
