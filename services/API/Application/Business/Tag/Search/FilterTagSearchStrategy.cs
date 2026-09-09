using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Tag.Search
{
    public class FilterTagSearchStrategy : FilteredSearchStrategyBase<TagEntity, EntitySearchCriteria>
    {
        protected override IQueryable<TagEntity> ApplyKeywordFilter(
        IQueryable<TagEntity> q, string keyword)
        => q.Where(e => e.Name.Contains(keyword));
    }
}
