using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Category.Search;

public sealed class AllCategorySearchStrategy(IHttpContextAccessor httpContext)
    : AllSearchStrategyBase<CategoryEntity, EntitySearchCriteria>(httpContext)
{
}
