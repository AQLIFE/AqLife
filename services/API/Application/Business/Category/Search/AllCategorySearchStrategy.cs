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
    //public override IQueryable<CategoryEntity> Execute(IQueryable<CategoryEntity> queryable, EntitySearchCriteria c, CancellationToken ct = default)
    //=> IsValid ? queryable : queryable.Skip(0).Take(10);// 仅返回前10条分类的查询记录，避免未认证用户获取全部数据 
}
