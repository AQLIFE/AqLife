using AqLife.Domain.Command;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqLife.Application.Services
{
    public class ScheduledPublishWorker(IServiceScopeFactory scopeFactory, ILogger<ScheduledPublishWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Scheduled publish worker started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogDebug("DateTimeOffset.UTCnow: {DateTimeOffsetUtcNow}", DateTimeOffset.UtcNow);
                logger.LogDebug("DateTimeOffset.now: {DateTimeOffsetNow}", DateTimeOffset.Now);
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var mediator = scope.ServiceProvider
                        .GetRequiredService<IMediator>();

                    var result =await mediator.Send(new ProcessScheduledPostsCommand(),stoppingToken);
                    logger.LogInformation("Scheduled posts processed. Found: {Found}, Published: {Published}, Failed: {Failed}", result.Found, result.Published, result.Failed);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,"Scheduled publish processing failed.");
                }
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }

            logger.LogInformation("Scheduled publish worker stopped.");
        }
    }
}

