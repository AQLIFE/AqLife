using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using MediatR;

namespace AqLife.Application.Business.Favorite.Handler
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
