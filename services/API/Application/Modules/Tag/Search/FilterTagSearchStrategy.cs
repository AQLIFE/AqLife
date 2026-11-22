using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;

namespace Tag.Search
{
    public sealed class FilterTagSearchStrategy : FilteredSearchStrategyBase<TagEntity, EntitySearchCriteria>
    {
        protected override IQueryable<TagEntity> ApplyKeywordFilter(
        IQueryable<TagEntity> q, string keyword)
        => q.Where(e => e.Name.Contains(keyword));
    }
}
