using AqLife.Application.Abstractions.Contracts.File;
using Microsoft.Extensions.Logging;

namespace File.Service;

/// <summary>
/// 定时发布应用用例。业务处理位于 Application，定时触发器由 Infrastructure 承担。
/// </summary>
public sealed class ScheduledPublishProcessor(
    IScheduledFileQuery scheduledFileQuery,
    TimeProvider timeProvider,
    IFilePublishService filePublishService,
    ILogger<ScheduledPublishProcessor> logger) : IScheduledPublishProcessor
{
    public async Task<ScheduledPublishResult> ProcessDueAsync(
        CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();
        var fileIds = await scheduledFileQuery.GetDueFileIdsAsync(now, cancellationToken);

        var published = 0;
        var failed = 0;

        foreach (var fileId in fileIds)
        {
            try
            {
                await filePublishService.PublishAsync(fileId, cancellationToken);
                logger.LogInformation("Published scheduled file with UID: {FileId}", fileId);
                published++;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to publish scheduled file with UID: {FileId}", fileId);
                failed++;
            }
        }

        return new ScheduledPublishResult(fileIds.Count, published, failed);
    }
}
