using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Exceptions;
using Favorite;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Favorite.Handler;

public class DeleteFavoriteHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteFavoriteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteFavoriteCommand command, CancellationToken ct)
    {
        var entity = await dbContext.FavoriteItems.SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Favorite 不存在");
        dbContext.FavoriteItems.Remove(entity);
        return Unit.Value;
    }
}
