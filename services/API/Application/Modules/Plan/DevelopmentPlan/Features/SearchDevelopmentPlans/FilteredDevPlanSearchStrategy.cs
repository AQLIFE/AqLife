using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;

namespace AqLife.Application.Plan.DevelopmentPlan.Search
{
    public class FilteredDevPlanSearchStrategy : FilteredSearchStrategyBase<DevelopmentPlanEntity, EntitySearchCriteria>
    {
        protected override IQueryable<DevelopmentPlanEntity> ApplyKeywordFilter(IQueryable<DevelopmentPlanEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
    }
}
