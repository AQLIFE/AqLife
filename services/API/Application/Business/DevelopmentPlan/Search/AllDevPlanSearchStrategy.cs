using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public class AllDevPlanSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<DevelopmentPlanEntity, EntitySearchCriteria>(httpContext)
    {
    }
}
