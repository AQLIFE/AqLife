using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqLife.Application.BackServices
{
    public class ScheduledPublishWorker(IServiceScopeFactory scopeFactory, ILogger<ScheduledPublishWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[Serilog][{@LogType}]=>{@LogDesc}",BehavioralLevel.ServiceType,"Scheduled publish worker started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var mediator = scope.ServiceProvider
                        .GetRequiredService<IMediator>();
                    var context = scope.ServiceProvider
                        .GetRequiredService<IApplicationDbContext>();
                    await using var transaction = await context.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        var result = await mediator.Send(new ProcessScheduledPostsCommand(), stoppingToken);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        logger.LogInformation("[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ServiceType,$"Scheduled posts processed. Found: {result.Found}, Published: {result.Published}, Failed: {result.Failed}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"后台任务执行失败:{ex}");
                        await transaction.RollbackAsync();
                    }
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Scheduled publish processing failed.");
                }
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }

            logger.LogInformation("Scheduled publish worker stopped.");
        }
    }
}

