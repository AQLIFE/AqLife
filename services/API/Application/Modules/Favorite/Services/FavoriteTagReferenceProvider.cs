using AqLife.Application.Abstractions.Contracts.Tag;
using AqLife.Application.Abstractions.Persistence.Modules;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite;

/// <summary>
/// Favorite 模块拥有的 Tag 引用检查。
/// </summary>
public sealed class FavoriteTagReferenceProvider(IFavoritePersistence persistence) : ITagReferenceProvider
{
    public Task<bool> IsReferencedAsync(Guid tagId, CancellationToken cancellationToken = default)
        => persistence.FavoriteItems.SelectMany(favorite => favorite.Tags)
            .AnyAsync(favoriteTag => favoriteTag.TagId == tagId, cancellationToken);
}
