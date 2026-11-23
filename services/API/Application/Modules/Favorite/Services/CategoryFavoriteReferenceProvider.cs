using AqLife.Application.Abstractions.Contracts.Category;
using AqLife.Application.Abstractions.Persistence.Modules;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite;

/// <summary>
/// Favorite 模块拥有的分类引用检查。
/// </summary>
public sealed class CategoryFavoriteReferenceProvider(IFavoritePersistence persistence)
    : ICategoryReferenceProvider
{
    public Task<bool> IsReferencedAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => persistence.FavoriteItems.AnyAsync(favorite => favorite.CategoryUID == categoryId, cancellationToken);
}
