using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Category.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Handler;

public sealed class CategoryQueryHandler(
    CategorySearch categorySearch,
    IApplicationDbContext dbContext,
    MapperServices mapperServices)
    : IRequestHandler<CategoryQuery, PageResult<CategoryDto>?>
{
    public async Task<PageResult<CategoryDto>?> Handle(CategoryQuery query, CancellationToken ct)
    {
        var shouldDesensitize = mapperServices.ShouldDesensitize;
        var allowedBlogExtensions = mapperServices.AllowedBlogExtensions;
        var categories = await categorySearch.SearchAsync(query, ct);

        // 先投影可由 EF Core 翻译的标量字段和统计值，再按统计值排序。
        // 不在 IQueryable 中构造 CategoryDto，避免 EF Core 尝试翻译 DTO 构造后的属性访问。
        var rows = await categories
            .Select(category => new
            {
                category.UID,
                category.CategoryName,
                category.CategoryDescription,
                CategoryCount = query.SearchType == CategoryQueryType.Blog
                    ? (shouldDesensitize
                        ? dbContext.File.Count(file =>
                            file.CategoryUID == category.UID &&
                            !file.IsTemplate &&
                            allowedBlogExtensions.Contains(file.Extension) &&
                            file.PublishMeta.PublishStatus == FileStatus.Published)
                        : dbContext.File.Count(file => file.CategoryUID == category.UID))
                    : (shouldDesensitize
                        ? dbContext.FavoriteItems.Count(favorite =>
                            favorite.CategoryUID == category.UID &&
                            favorite.IsPublished)
                        : dbContext.FavoriteItems.Count(favorite => favorite.CategoryUID == category.UID)),
                ParentName = category.ParentCategory == null
                    ? null
                    : category.ParentCategory.CategoryName
            })
            .OrderByDescending(category => category.CategoryCount)
            .Take(10)
            .ToListAsync(ct);

        var result = mapperServices.ToDtos(rows.Select(category => new CategoryMappingData(
            category.UID,
            category.CategoryName,
            category.CategoryDescription,
            category.CategoryCount,
            category.ParentName)));

        return new PageResult<CategoryDto>(result, 1, 10, false);
    }
}
