using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Modules.Tag.Search
{
    public sealed class AllTagSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<TagEntity, EntitySearchCriteria>(httpContext)
    {

    }
}
