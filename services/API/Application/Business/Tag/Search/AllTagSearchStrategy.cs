using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Tag.Search
{
    public sealed class AllTagSearchStrategy(IHttpContextAccessor httpContext): AllSearchStrategyBase<TagEntity, EntitySearchCriteria>(httpContext)
    {
        
    }
}
