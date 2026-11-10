using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AqLife.Application.Business.Category.Search;

public sealed class AllCategorySearchStrategy(IHttpContextAccessor httpContext)
    : AllSearchStrategyBase<CategoryEntity, EntitySearchCriteria>(httpContext)
{
}
