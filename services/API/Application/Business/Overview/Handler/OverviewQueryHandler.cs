using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.Overview.Handler
{
    public class OverviewQueryHandler(
        IApplicationDbContext dbContext,
        IOptions<FilePolicyOption> filePolicy) : IRequestHandler<OverviewQuery, OverviewDto>
    {
        public async Task<OverviewDto> Handle(OverviewQuery query, CancellationToken ct)
        {
            var files = dbContext.File
                .AsNoTracking()
                .Where(file => !file.IsTemplate)
                .Include(file => file.PublishMeta)
                .Include(file => file.InteractionMeta)
                .Include(file => file.FileTags)
                    .ThenInclude(fileTag => fileTag.Tag);

            var draftCount = await files.CountAsync(file => file.PublishMeta.PublishStatus == FileStatus.Draft, ct);
            var scheduledCount = await files.CountAsync(file => file.PublishMeta.PublishStatus == FileStatus.Scheduled, ct);
            var publishedCount = await files.CountAsync(file => file.PublishMeta.PublishStatus == FileStatus.Published, ct);

            var recentFiles = await files
                .OrderByDescending(file => file.UploadTime)
                .Take(Math.Clamp(query.RecentCount, 1, 20))
                .Select(file => new FileDto(
                    file.UID,
                    file.FileName,
                    file.FileTags.Select(fileTag => new TagDto(
                        fileTag.Tag.UID.ToString(),
                        fileTag.Tag.Name,
                        fileTag.Tag.AliasName,
                        fileTag.Tag.IsCategory)),
                    file.PublishMeta.PublishStatus,
                    file.PublishMeta.PublishAt,
                    file.FileSize,
                    file.FileHash,
                    file.UploadTime.ToString(),
                    file.Extension,
                    file.FileIntroduction,
                    file.InteractionMeta.ViewCount,
                    file.Version,
                    file.IsTemplate))
                .ToListAsync(ct);

            return new OverviewDto(
                draftCount,
                scheduledCount,
                publishedCount,
                recentFiles);
        }
    }
}
