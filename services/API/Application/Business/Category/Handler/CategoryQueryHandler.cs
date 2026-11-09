using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Category.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.Category.Handler;

public sealed class CategoryQueryHandler(
    CategorySearch categorySearch,
    IApplicationDbContext dbContext,
    IHttpContextAccessor httpContext,
    IOptions<FilePolicyOption> filePolicy)
    : IRequestHandler<CategoryQuery, PageResult<CategoryDto>?>
{
    private bool IsAuthenticated =>
        httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public async Task<PageResult<CategoryDto>?> Handle(CategoryQuery query, CancellationToken ct)
    {
        var isAuthenticated = IsAuthenticated;
        var allowedBlogExtensions = filePolicy.Value.AllowedBlogExtensions;
        var categories = await categorySearch.SearchAsync(query, ct);

        var result = await categories
            .Select(category => new CategoryDto(
                category.UID,
                category.CategoryName,
                category.CategoryDescription,
                query.SearchType == CategoryQueryType.Blog
                    ? (isAuthenticated
                        ? dbContext.File.Count(file => file.CategoryUID == category.UID)
                        : dbContext.File.Count(file =>
                            file.CategoryUID == category.UID &&
                            !file.IsTemplate &&
                            allowedBlogExtensions.Contains(file.Extension) &&
                            file.PublishMeta.PublishStatus == FileStatus.Published))
                    : (isAuthenticated
                        ? dbContext.FavoriteItems.Count(favorite => favorite.CategoryUID == category.UID)
                        : dbContext.FavoriteItems.Count(favorite =>
                            favorite.CategoryUID == category.UID && favorite.IsPublished)),
                category.ParentCategory == null ? null : category.ParentCategory.CategoryName))
            .OrderByDescending(category => category.CategoryCount)
            .Take(10)
            .ToListAsync(ct);

        return new PageResult<CategoryDto>(result, 1, 10, false);
    }
}
