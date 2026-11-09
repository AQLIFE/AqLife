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

            var mediaItems = (command.Media ?? [])
                .Where(x => x.FileId != Guid.Empty)
                .GroupBy(x => x.FileId)
                .Select(group => group.First());

            foreach (var item in mediaItems)
            {
                var role = item.Role is "Cover" or "Image" or "Attachment" ? item.Role : "Image";
                entity.Media.Add(new FavoriteMediaEntity(entity.UID, item.FileId, role, item.SortOrder));
            }

            var tagIds = (command.TagIds ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct();

            foreach (var tagId in tagIds)
                entity.Tags.Add(new FavoriteTagEntity(entity.UID, tagId));

            await dbContext.FavoriteItems.AddAsync(entity, ct);
            return entity.UID;
        }
    }
}
