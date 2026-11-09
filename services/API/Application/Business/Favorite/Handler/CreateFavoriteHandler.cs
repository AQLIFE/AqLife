using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.Favorite.Handler
{
    public class CreateFavoriteHandler(IApplicationDbContext dbContext, FavoriteMapper mapper) : IRequestHandler<CreateFavoriteCommand, Guid>
    {
        public async Task<Guid> Handle(CreateFavoriteCommand command, CancellationToken ct)
        {
            var entity = mapper.ToEntity(command);
            await dbContext.FavoriteItems.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
