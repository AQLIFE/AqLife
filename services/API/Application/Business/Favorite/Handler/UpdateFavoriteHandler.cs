using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite.Handler
{
    public class UpdateFavoriteHandler(IApplicationDbContext dbContext)
        : IRequestHandler<UpdateFavoriteCommand, Guid>, IRequestHandler<SetFavoritePublishedCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFavoriteCommand command, CancellationToken ct)
        {

            FavoriteItemEntity entity = await dbContext.FavoriteItems
                .Include(x => x.Media)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Favorite 不存在");

            entity.Update(command.Title, command.Description,command.SourceUrl, command.SortOrder, command.CID);

            entity.Media.Clear();
            foreach (var item in (command.Media ?? [])
                .Where(x => x.FileId != Guid.Empty)
                .GroupBy(x => x.FileId)
                .Select(group => group.First()))
            {
                //var role = item.Role is "Cover" or "Image" or "Attachment" ? item.Role : "Image";
                entity.Media.Add(new FavoriteMediaEntity(item.FileId,entity.UID,sortOrder:item.SortOrder));
            }

            entity.Tags.Clear();
            foreach (var tagId in (command.TagIds ?? [])
                .Where(id => id != Guid.Empty)
                .Distinct())
            {
                entity.Tags.Add(new FavoriteTagEntity(entity.UID, tagId));
            }

            return entity.UID;
        }

        public async Task<Guid> Handle(SetFavoritePublishedCommand command, CancellationToken ct)
        {
            var entity = await dbContext.FavoriteItems.SingleOrDefaultAsync(e => e.UID == command.UID, ct)?? throw new ResourceNotFoundException("Favorite 不存在");
            entity.SetPublished(command.IsPublished);
            return entity.UID;
        }
    }
}
