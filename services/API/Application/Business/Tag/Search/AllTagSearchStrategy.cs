using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Tag.Search
{
    public sealed class AllTagSearchStrategy
        : AllSearchStrategyBase<TagEntity, EntitySearchCriteria>
    {
        public override IQueryable<TagEntity> Execute(
            IQueryable<TagEntity> queryable,
            EntitySearchCriteria criteria,
            CancellationToken ct = default)
        {
            return queryable;
        }
    }
}
