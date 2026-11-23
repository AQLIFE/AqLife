using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Favorite;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Favorite.Handler;

public class DeleteFavoriteHandler(IFavoritePersistence persistence) : IRequestHandler<DeleteFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFavoriteCommand command, CancellationToken ct)
    {
        var entity = await persistence.FavoriteItems.SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Favorite 不存在");
        persistence.RemoveFavorite(entity);
        return Unit.Value;
    }
}