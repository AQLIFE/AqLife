using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence.Modules;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite;

/// <summary>
/// Favorite 模块拥有的文件引用检查：收藏内容关联的媒体文件。
/// </summary>
public sealed class FavoriteFileReferenceProvider(IFavoritePersistence persistence)
    : IFileReferenceProvider
{
    public Task<bool> IsReferencedAsync(Guid fileId, CancellationToken cancellationToken = default)
        => persistence.FavoriteItems.SelectMany(item => item.Media)
            .AnyAsync(media => media.FileId == fileId, cancellationToken);
}
