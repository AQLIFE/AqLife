using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.File;
using AqLife.Application.File.Service;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Core.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;

namespace AqLife.Application.File.Handler
{
    public class FileQueryHandler(ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity> search, IOptions<FilePolicyOption> options, FileReader fileReader, PageResultMapper<FileMetaEntity, FileDto> mapper) : IRequestHandler<FileQuery, PageResult<FileDto>>
    {
        public async Task<PageResult<FileDto>> Handle(FileQuery query, CancellationToken ct)
        {
            var result = await search.SearchPageAsync(query, ct);
            if (query.Order == FileOrder.Latest)
            {
                var items = result.Items.ToList();
                foreach (var item in items)
                {
                    if (options.Value.AllowedBlogExtensions.Contains(item.Extension))
                    {
                        string content = await fileReader.GetContentAsync(item.StorageKey, ct);
                        item.SetFileIntroduction(content.Length <= 300 ? content : content[..300]);
                    }
                }
            }
            return mapper.ToDto(result);
        }
    }
    public class FileOverviewQueryHandler(IFilePersistence persistence, IViewMapper<FileMetaEntity, FileDto> fileMapper) : IRequestHandler<FileOverviewQuery, FileOverviewDto>
    {
        public async Task<FileOverviewDto> Handle(FileOverviewQuery query, CancellationToken ct)
        {
            IIncludableQueryable<FileMetaEntity, TagEntity> filequery = persistence.Files.AsNoTracking()
                .Where(file => !file.IsTemplate)
                .Include(file => file.Category)
                .Include(file => file.PublishMeta)
                .Include(file => file.InteractionMeta)
                .Include(file => file.FileTags)
                    .ThenInclude(fileTag => fileTag.Tag);

            IQueryable<FileMetaEntity> draft = filequery.Where(e => e.PublishMeta.PublishStatus == FileStatus.Draft);
            IQueryable<FileMetaEntity> scheduled = filequery.Where(e => e.PublishMeta.PublishStatus == FileStatus.Scheduled);
            IQueryable<FileMetaEntity> published = filequery.Where(e => e.PublishMeta.PublishStatus == FileStatus.Published);
            var draftCount = await draft.CountAsync(ct);
            var scheduledCount = await scheduled.CountAsync(ct);
            var publishedCount = await published.CountAsync(ct);


            IQueryable<FileMetaEntity> result = query.PublishStatus switch
            {
                PublishStatusSearch.Default => filequery,
                PublishStatusSearch.Draft => draft,
                PublishStatusSearch.Scheduled => scheduled,
                PublishStatusSearch.Published => published,
                _ => throw new RequestFailException("不存在的查询条件")
            };
            result = result.OrderByDescending(file => file.UploadTime).Take(Math.Clamp(query.RecentCount, 1, 10));

            return new FileOverviewDto(draftCount, scheduledCount, publishedCount, [.. result.Select(fileMapper.ToDto)], 1, query.RecentCount, query.PublishStatus, false);
        }
    }
}
