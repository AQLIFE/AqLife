using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite.Handler;

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
