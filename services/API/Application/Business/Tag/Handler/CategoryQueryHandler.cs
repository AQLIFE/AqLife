using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.Category.Handler
{
    public class CategoryQueryHandler(
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

            IQueryable<CategoryDto> categories = dbContext.Categories
                .AsNoTracking()
                .Where(category => query.CID == null || category.UID == query.CID)
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
                .Take(10);

            var result = await categories.ToListAsync(ct);
            return new PageResult<CategoryDto>(result, 1, 10, false);
        }
    }
}
