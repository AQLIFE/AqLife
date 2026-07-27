using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mapper;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.IView;
namespace MyLife.Application.Business.Account.Search
{
    // 尝试使用BaseSearch来实现AccountSearch，减少重复代码,等待测试
    public class AccountSearch(QueryMapper queryMapper, AppStorage storage, IEnumerable<ISearchStrategy<AccountEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<AccountQuery, AccountEntity, AccountDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override async Task<IQueryable<AccountEntity>> BuildBaseQueryAsync(IQueryable<AccountEntity> queryable, AccountQuery query)
        => queryable.Include(e => e.Subscriptions);

        protected override EntitySearchCriteria MapToCriteria(AccountQuery query)
        => queryMapper.ToCriteria(query);
    }
}
