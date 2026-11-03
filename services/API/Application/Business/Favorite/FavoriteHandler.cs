using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.Favorite;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Favorite;

public class FavoriteHandler(IApplicationDbContext storage) :
    IRequestHandler<FavoriteQuery, PageResult<FavoriteDto>>,
    IRequestHandler<CreateFavoriteCommand, Guid>,
    IRequestHandler<UpdateFavoriteCommand, Guid>,
    IRequestHandler<SetFavoritePublishedCommand, Guid>,
    IRequestHandler<DeleteFavoriteCommand, Unit>
{
    public async Task<PageResult<FavoriteDto>> Handle(FavoriteQuery query, CancellationToken ct)
    {
        var items = storage.Set<FavoriteItemEntity>().AsNoTracking()
            .Include(x => x.Media).Include(x => x.Tags).AsQueryable();

        if (!query.IncludeUnpublished)
            items = items.Where(x => x.IsPublished);
        if (query.UID.HasValue)
            items = items.Where(x => x.UID == query.UID.Value);
        if (!string.IsNullOrWhiteSpace(query.ContentType))
            items = items.Where(x => x.ContentType == query.ContentType);
        if (query.TagId.HasValue)
            items = items.Where(x => x.Tags.Any(t => t.TagId == query.TagId.Value));

        var total = await items.CountAsync(ct);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var rows = await items.OrderBy(x => x.SortOrder).ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var data = rows.Select(ToDto).ToList();
        return new PageResult<FavoriteDto>(data, page, pageSize, page * pageSize < total);
    }

    public async Task<Guid> Handle(CreateFavoriteCommand command, CancellationToken ct)
    {
        var entity = new FavoriteItemEntity(command.Title, command.Description ?? string.Empty,
            command.ContentType, command.SourceUrl, command.SortOrder);
        await storage.Set<FavoriteItemEntity>().AddAsync(entity, ct);
        await ReplaceRelations(entity, command.FileIds, command.TagIds, ct);
        return entity.UID;
    }

    public async Task<Guid> Handle(UpdateFavoriteCommand command, CancellationToken ct)
    {
        var entity = await storage.Set<FavoriteItemEntity>().Include(x => x.Media).Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.UID == command.UID, ct)
            ?? throw new ResourceNotFoundException("Favorite 不存在");
        entity.Update(command.Title, command.Description ?? string.Empty, command.ContentType,
            command.SourceUrl, command.SortOrder);
        storage.Set<FavoriteMediaEntity>().RemoveRange(entity.Media);
        storage.Set<FavoriteTagEntity>().RemoveRange(entity.Tags);
        entity.Media.Clear();
        entity.Tags.Clear();
        await ReplaceRelations(entity, command.FileIds, command.TagIds, ct);
        return entity.UID;
    }

    public async Task<Guid> Handle(SetFavoritePublishedCommand command, CancellationToken ct)
    {
        var entity = await storage.Set<FavoriteItemEntity>().FindAsync([command.UID], ct)
            ?? throw new ResourceNotFoundException("Favorite 不存在");
        entity.SetPublished(command.IsPublished);
        return entity.UID;
    }

    public async Task<Unit> Handle(DeleteFavoriteCommand command, CancellationToken ct)
    {
        var entity = await storage.Set<FavoriteItemEntity>().FindAsync([command.UID], ct)
            ?? throw new ResourceNotFoundException("Favorite 不存在");
        storage.Set<FavoriteItemEntity>().Remove(entity);
        return Unit.Value;
    }

    private async Task ReplaceRelations(FavoriteItemEntity entity, IEnumerable<Guid>? fileIds,
        IEnumerable<Guid>? tagIds, CancellationToken ct)
    {
        var files = (fileIds ?? []).Where(x => x != Guid.Empty).Distinct().ToArray();
        var tags = (tagIds ?? []).Where(x => x != Guid.Empty).Distinct().ToArray();

        if (files.Length > 0)
        {
            var existingFiles = await storage.File.Where(x => files.Contains(x.UID)).Select(x => x.UID).ToListAsync(ct);
            if (existingFiles.Count != files.Length)
                throw new ResourceNotFoundException("一个或多个关联文件不存在");
            foreach (var fileId in files)
                entity.Media.Add(new FavoriteMediaEntity(entity.UID, fileId, "Image", entity.Media.Count));
        }

        if (tags.Length > 0)
        {
            var existingTags = await storage.Tags.Where(x => tags.Contains(x.UID)).Select(x => x.UID).ToListAsync(ct);
            if (existingTags.Count != tags.Length)
                throw new ResourceNotFoundException("一个或多个关联标签不存在");
            foreach (var tagId in tags)
                entity.Tags.Add(new FavoriteTagEntity(entity.UID, tagId));
        }
    }

    private static FavoriteDto ToDto(FavoriteItemEntity entity) => new(
        entity.UID, entity.Title, entity.Description, entity.ContentType, entity.SourceUrl,
        entity.SortOrder, entity.IsPublished, entity.CreatedAt, entity.UpdatedAt,
        entity.Media.OrderBy(x => x.SortOrder).Select(x => new FavoriteMediaDto(x.FileId, x.Role, x.SortOrder)).ToList(),
        entity.Tags.Select(x => x.TagId).ToList());
}
