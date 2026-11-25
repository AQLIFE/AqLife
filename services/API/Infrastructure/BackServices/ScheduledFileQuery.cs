using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure.BackServices;

/// <summary>
/// 定时发布查询的 EF Core 适配器。
/// </summary>
public sealed class ScheduledFileQuery(IFilePersistence persistence) : IScheduledFileQuery
{
    public async Task<IReadOnlyList<Guid>> GetDueFileIdsAsync(
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default)
        => await persistence.Files
            .Where(file => file.PublishMeta.PublishStatus == FileStatus.Scheduled
                && file.PublishMeta.PublishAt <= asOf)
            .Select(file => file.UID)
            .ToListAsync(cancellationToken);
}
