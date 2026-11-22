using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.Favorite;
using Favorite;
using MediatR;

namespace Favorite.Handler
{
    public class CreateFavoriteHandler(IApplicationDbContext dbContext, ICreateMapper<FavoriteItemEntity, CreateFavoriteCommand> mapper)
        : IRequestHandler<CreateFavoriteCommand, Guid>
    {
        public async Task<Guid> Handle(CreateFavoriteCommand command, CancellationToken ct)
        {
            var entity = mapper.ToEntity(command);
            await dbContext.FavoriteItems.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
