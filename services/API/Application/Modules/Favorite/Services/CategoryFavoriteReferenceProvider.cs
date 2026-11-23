using AqLife.Application.Abstractions.Contracts.Category;
using AqLife.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite;

/// <summary>
/// Favorite 模块拥有的分类引用检查。
/// </summary>
public sealed class CategoryFavoriteReferenceProvider(IApplicationDbContext dbContext)
    : ICategoryReferenceProvider
{
    public Task<bool> IsReferencedAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
        => dbContext.FavoriteItems.AnyAsync(
            favorite => favorite.CategoryUID == categoryId,
            cancellationToken);
}