using AqLife.Domain.Entities.Favorite;

namespace AqLife.Application.Abstractions.Persistence.Modules;

/// <summary>
/// Favorite 模块专属持久化入口。
/// </summary>
public interface IFavoritePersistence
{
    IQueryable<FavoriteItemEntity> FavoriteItems { get; }
    Task AddFavoriteAsync(FavoriteItemEntity favorite, CancellationToken cancellationToken = default);
    void RemoveFavorite(FavoriteItemEntity favorite);
}
