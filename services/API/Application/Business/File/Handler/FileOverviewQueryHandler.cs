using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace AqLife.Application.Business.File.Handler
{
    public class FileOverviewQueryHandler(IApplicationDbContext dbContext, IViewMapper<FileMetaEntity, FileDto> fileMapper) : IRequestHandler<FileOverviewQuery, FileOverviewDto>
    {
        public async Task<FileOverviewDto> Handle(FileOverviewQuery query, CancellationToken ct)
        {
            IIncludableQueryable<FileMetaEntity,TagEntity> filequery = dbContext.File.AsNoTracking().Where(file => !file.IsTemplate).Include(file => file.PublishMeta).Include(file => file.InteractionMeta).Include(file => file.FileTags)
                    .ThenInclude(fileTag => fileTag.Tag);

            IQueryable<FileMetaEntity> draft = filequery.Where(e=>e.PublishMeta.PublishStatus == FileStatus.Draft);
            IQueryable<FileMetaEntity> scheduled = filequery.Where(e=>e.PublishMeta.PublishStatus == FileStatus.Scheduled);
            IQueryable<FileMetaEntity> published = filequery.Where(e => e.PublishMeta.PublishStatus == FileStatus.Published);
            var draftCount = await draft.CountAsync(ct);
            var scheduledCount = await scheduled.CountAsync(ct);
            var publishedCount = await published.CountAsync(ct);


            IQueryable<FileMetaEntity> result = query.PublishStatus switch
            {
                PublishStatusSearch.Default => filequery.OrderByDescending(file => file.UploadTime),
                PublishStatusSearch.Draft => draft,
                PublishStatusSearch.Scheduled => scheduled,
                PublishStatusSearch.Published => published,
                _ => throw new RequestFailException("不存在的查询条件")
            };

            return new FileOverviewDto(draftCount,scheduledCount,publishedCount,[..result.Select(fileMapper.ToDto)],1,query.RecentCount,query.PublishStatus,false);
        }
    }
}
