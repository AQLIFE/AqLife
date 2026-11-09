using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Overview.Handler
{
    public class OverviewQueryHandler(IApplicationDbContext dbContext, FileMappingService fileMapper) : IRequestHandler<OverviewQuery, OverviewDto>
    {
        public async Task<OverviewDto> Handle(OverviewQuery query, CancellationToken ct)
        {
            var filequery = dbContext.File
                .AsNoTracking()
                .Where(file => !file.IsTemplate)
                .Include(file => file.PublishMeta)
                .Include(file => file.InteractionMeta)
                .Include(file => file.FileTags)
                    .ThenInclude(fileTag => fileTag.Tag);

            var group = filequery.GroupBy(file => file.PublishMeta.PublishStatus); // 分组可以减少数据库查询次数，避免多次查询数据库

            var draftCount = await group.CountAsync(item => item.Key == FileStatus.Draft, ct);
            var scheduledCount = await group.CountAsync(item => item.Key == FileStatus.Scheduled, ct);
            var publishedCount = await group.CountAsync(item => item.Key == FileStatus.Published, ct);



            var recentFiles = await filequery
                .OrderByDescending(file => file.UploadTime)
                .Take(Math.Clamp(query.RecentCount, 1, 20))
                .Select(file => fileMapper.ToDto(file))
                .ToListAsync(ct);

            return new OverviewDto(
                draftCount,
                scheduledCount,
                publishedCount,
                recentFiles);
        }
    }
}
