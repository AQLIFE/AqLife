using AqLife.Application.Services;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqLife.Application.Business.File.Handler;
public class ProcessScheduledPostsHandler(
    BlogSearch search,
    TimeProvider timeProvider,
    IBlogPublishService service,
    ILogger<ProcessScheduledPostsHandler> logger)
    : IRequestHandler<ProcessScheduledPostsCommand, ProcessScheduledPostsResult>
{
	public async Task<ProcessScheduledPostsResult> Handle(ProcessScheduledPostsCommand command, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow();
        var scheduledPosts = await search.GetScheduledPostsDueAsync(now, ct);
        ProcessScheduledPostsResult PublishResult = new(scheduledPosts.Count(), 0, 0);
        foreach (var post in scheduledPosts)
        {
            try
            {
                await service.PublishAsync(post.UID, ct);
                logger.LogInformation("Published scheduled post with UID: {PostUID}", post.UID);
                PublishResult.Published++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish scheduled post with UID: {PostUID}", post.UID);
                PublishResult.Failed++;
            }
        }
        return PublishResult;
    }
}
