using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public class FilteredDevPlanSearchStrategy : FilteredSearchStrategyBase<DevelopmentPlanEntity, EntitySearchCriteria>
    {
        private protected override IQueryable<DevelopmentPlanEntity> ApplyKeywordFilter(IQueryable<DevelopmentPlanEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
    }
}
