using AqLife.Domain.Entities;
using Core.Search;

namespace AqLife.Application.Account.Search
{
    public sealed class ValidAccount : FilteredSearchStrategyBase<AccountEntity, EntitySearchCriteria>
    {
        protected override IQueryable<AccountEntity> ApplyKeywordFilter(IQueryable<AccountEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
    }
}
