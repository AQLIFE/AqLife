using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Plan.DevelopmentPlan.Search
{
    public class AllDevPlanSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<DevelopmentPlanEntity, EntitySearchCriteria>(httpContext)
    {
    }
}
