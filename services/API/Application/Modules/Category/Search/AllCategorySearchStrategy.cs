using AqLife.Domain.Entities;
using Core.Search;
using Microsoft.AspNetCore.Http;

namespace Category.Search;

public sealed class AllCategorySearchStrategy(IHttpContextAccessor httpContext)
    : AllSearchStrategyBase<CategoryEntity, EntitySearchCriteria>(httpContext)
{
}
