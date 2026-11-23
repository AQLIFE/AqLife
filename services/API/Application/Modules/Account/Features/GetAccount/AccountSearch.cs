using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Account;
using AqLife.Domain.Entities;
using Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Account.Search
{
    // 尝试使用BaseSearch来实现AccountSearch，减少重复代码,等待测试
    public sealed class AccountSearch(ISearchMapper<AccountQuery, AccountDto, EntitySearchCriteria> queryMapper/*无法解耦*/, IApplicationDbContext dbContext, IHttpContextAccessor httpContext, IEnumerable<ISearchStrategy<AccountEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<AccountQuery, AccountDto, AccountEntity, EntitySearchCriteria>(dbContext, httpContext, searchStrategies)
    {
        protected override IQueryable<AccountEntity> BuildBaseQuery(IQueryable<AccountEntity> queryable, AccountQuery query)
        => queryable.Include(e => e.Subscriptions);

        protected override EntitySearchCriteria MapToCriteria(AccountQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<AccountEntity> ApplyDefaultOrder(IQueryable<AccountEntity> queryble, AccountQuery query)
        => queryble.OrderBy(e => e.Name);

        protected override IQueryable<AccountEntity> ApplyResultPolicy(IQueryable<AccountEntity> queryable, AccountQuery query)
        => queryable.Where(e => e.IsValid);
    }
}
