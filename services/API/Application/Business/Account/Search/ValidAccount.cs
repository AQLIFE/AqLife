using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Account.Search
{
    public sealed class ValidAccount : FilteredSearchStrategyBase<AccountEntity, EntitySearchCriteria>
    {
        protected private override IQueryable<AccountEntity> ApplyKeywordFilter(IQueryable<AccountEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
    }
}
