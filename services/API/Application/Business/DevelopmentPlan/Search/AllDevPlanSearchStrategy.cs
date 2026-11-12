using AqLife.Application.Search;
using AqLife.Domain.Entities.DevPlan;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.DevelopmentPlan.Search
{
    public class AllDevPlanSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<DevelopmentPlanEntity,EntitySearchCriteria>(httpContext)
    {
    }
}
