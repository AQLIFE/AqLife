using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Account.Search
{
    // 尝试使用BaseSearch来实现AccountSearch，减少重复代码,等待测试
    public class AccountSearch(QueryMapper queryMapper, IApplicationDbContext storage, IHttpContextAccessor httpContext, IEnumerable<ISearchStrategy<AccountEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<AccountQuery, AccountEntity, AccountDto, EntitySearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected private override IQueryable<AccountEntity> BuildBaseQuery(IQueryable<AccountEntity> queryable, AccountQuery query)
        => queryable.Include(e => e.Subscriptions);

        protected private override EntitySearchCriteria MapToCriteria(AccountQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<AccountEntity> ApplyDefaultOrder(IQueryable<AccountEntity> queryble, AccountQuery query)
        => queryble.OrderBy(e => e.Name);
    }
}
