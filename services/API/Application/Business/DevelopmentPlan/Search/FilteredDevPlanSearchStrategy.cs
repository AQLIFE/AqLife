using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public class FilteredDevPlanSearchStrategy :FilteredSearchStrategyBase<DevelopmentPlanEntity,EntitySearchCriteria>
    {
        private protected override IQueryable<DevelopmentPlanEntity> ApplyKeywordFilter(IQueryable<DevelopmentPlanEntity> queryable, string keyword)
        => queryable.Where(e => e.Name.Contains(keyword));
    }
}
