using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using MediatR;

namespace AqLife.Application.Business.Favorite.Handler
{
    public class CreateFavoriteHandler(IApplicationDbContext dbContext, FavoriteMapper mapper)
        : IRequestHandler<CreateFavoriteCommand, Guid>
    {
        public async Task<Guid> Handle(CreateFavoriteCommand command, CancellationToken ct)
        {
            var entity = mapper.ToEntity(command);

            IEnumerable<FavoriteMediaEntity>? medias = command.Media?.Distinct().Select(e => new FavoriteMediaEntity(favoriteId: entity.UID, fileId: e));
            IEnumerable<FavoriteTagEntity>? tags = command.TagIds?.Distinct().Select(e => new FavoriteTagEntity(favoriteId: entity.UID, tagId: e));

            if(medias is not null) foreach (FavoriteMediaEntity file in medias) entity.Media.Add(file);
            if (tags is not null) foreach (FavoriteTagEntity tag in tags) entity.Tags.Add(tag);

            await dbContext.FavoriteItems.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
