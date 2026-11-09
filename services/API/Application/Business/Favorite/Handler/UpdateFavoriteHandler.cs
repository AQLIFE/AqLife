using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite.Handler
{
    public class UpdateFavoriteHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateFavoriteCommand, Guid>, IRequestHandler<SetFavoritePublishedCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFavoriteCommand command, CancellationToken ct)
        {
            var entity = await dbContext.FavoriteItems.Include(x => x.Media).Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Favorite 不存在");
            entity.Update(command.Title, command.Description ?? string.Empty,
                command.SourceUrl, command.SortOrder, command.CID);
            entity.Media.Clear();
            entity.Tags.Clear();

            await dbContext.FavoriteTags.AddRangeAsync(entity.Tags, ct);
            await dbContext.FavoriteMedia.AddRangeAsync(entity.Media, ct);

            return entity.UID;
        }

        public async Task<Guid> Handle(SetFavoritePublishedCommand command, CancellationToken ct)
        {
            var entity = await dbContext.FavoriteItems.SingleOrDefaultAsync(e => e.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Favorite 不存在");
            entity.SetPublished(command.IsPublished);
            return entity.UID;
        }
    }
}
