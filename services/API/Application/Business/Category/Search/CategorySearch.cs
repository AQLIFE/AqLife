using System.Linq.Expressions;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.Category.Search;

//public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword = null) : ISearchCriteria;

public sealed class CategorySearch(
    IApplicationDbContext dbContext, QueryMapper queryMapper, IHttpContextAccessor httpContext, IOptions<FilePolicyOption> options,
    IEnumerable<ISearchStrategy<CategoryEntity, EntitySearchCriteria>> strategies) : BaseSearch<CategoryQuery, CategoryEntity, CategoryDto, EntitySearchCriteria>(dbContext, httpContext, strategies)
{
    private static readonly Expression<Func<CategoryEntity, CategoryEntity>>
        AnonymousProjection = category => Desensitize(category);

    protected private override IQueryable<CategoryEntity> ApplyResultPolicy(
        IQueryable<CategoryEntity> queryable, CategoryQuery query)
        => IsValid || query.SearchType != CategoryQueryType.File
            ? queryable
            : queryable.Select(AnonymousProjection);

    private static CategoryEntity Desensitize(CategoryEntity category)
    {
        foreach (var file in category.Files)
        {
            file.Desensitize();
        }

        return category;
    }

    protected private override EntitySearchCriteria MapToCriteria(CategoryQuery query)
        => queryMapper.ToCriteria(query);

    protected private override IQueryable<CategoryEntity> ApplyDefaultOrder(IQueryable<CategoryEntity> queryble, CategoryQuery query)
        => queryble.OrderBy(x => x.CategoryName);

    protected private override IQueryable<CategoryEntity> BuildPageQuery(IQueryable<CategoryEntity> queryable, IPageQuery pageQuery)
        => IsValid ? base.BuildPageQuery(queryable, pageQuery) : queryable.Skip(0).Take(10); // 未认证用户只能看到前10条记录

    protected private override IQueryable<CategoryEntity> BuildBaseQuery(IQueryable<CategoryEntity> queryable, CategoryQuery query)
    {
        queryable = queryable.Include(p => p.ParentCategory); // include 关键导航属性
        queryable = query.SearchType switch
        {
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
        // 没有Include ,则对应的Count都为0

        return queryable;
    }
}