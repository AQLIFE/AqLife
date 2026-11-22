using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Category;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Category.Search;

//public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword = null) : ISearchCriteria;

public sealed class CategorySearch(
    IApplicationDbContext dbContext, ISearchMapper<CategoryQuery, PageResult<CategoryDto>, EntitySearchCriteria> queryMapper, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options,
    IEnumerable<ISearchStrategy<CategoryEntity, EntitySearchCriteria>> strategies) :
    BaseSearch<CategoryQuery, PageResult<CategoryDto>, CategoryEntity, EntitySearchCriteria>(dbContext, httpContext, strategies)
{
    protected  override IQueryable<CategoryEntity> ApplyResultPolicy(
        IQueryable<CategoryEntity> queryable, CategoryQuery query)
        => query.SearchType switch
        {
            CategoryQueryType.Default => IsValid ? queryable.Include(e => e.Files).Include(e => e.Favorites) : queryable,
            CategoryQueryType.File => IsValid
                ? queryable.Include(e => e.Files) // include 所有文件,不用筛选发布状态,所以不再使用 ThenInclude
                : queryable
                    .Include(e => e.Files.Where(f =>
                        !f.IsTemplate && f.PublishMeta.PublishStatus == FileStatus.Published &&
                        options.Value.AllowedBlogExtensions.Contains(f.Extension)))
                    .ThenInclude(f => f.PublishMeta),

            CategoryQueryType.Favorite => IsValid
                ? queryable.Include(e => e.Favorites)
                : queryable.Include(e => e.Favorites.Where(f => f.IsPublished)),

            _ => throw new RequestFailException("Invalid search type")
        };

    protected  override EntitySearchCriteria MapToCriteria(CategoryQuery query)
            => queryMapper.ToCriteria(query);

    protected  override IQueryable<CategoryEntity> ApplyDefaultOrder(IQueryable<CategoryEntity> queryble, CategoryQuery query)
        => queryble.OrderByDescending(x => x.Files.Count+x.Favorites.Count);

    protected  override IQueryable<CategoryEntity> BuildPageQuery(IQueryable<CategoryEntity> queryable, IPageQuery pageQuery)
        => IsValid ? base.BuildPageQuery(queryable, pageQuery) : queryable.Skip(0).Take(10); // 未认证用户只能看到前10条记录

    protected  override IQueryable<CategoryEntity> BuildBaseQuery(IQueryable<CategoryEntity> queryable, CategoryQuery query)
    => queryable.Include(p => p.ParentCategory); // include 关键导航属性
}