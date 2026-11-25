using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqLife.Infrastructure.BackServices;

/// <summary>
/// 定时发布宿主适配器。只负责调度应用层处理器及提供事务边界，不承载发布业务规则。
/// </summary>
public sealed class ScheduledPublishWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<ScheduledPublishWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "[Serilog][{@LogType}]=>{@LogDesc}",
            BehavioralLevel.ServiceType,
            "Scheduled publish worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var services = scope.ServiceProvider;

                var processor = services.GetRequiredService<IScheduledPublishProcessor>();
                var dbContext = services.GetRequiredService<IUnitOfWork>();

                await using var transaction = await dbContext.BeginTransactionAsync(stoppingToken);
                try
                {
                    var result = await processor.ProcessDueAsync(stoppingToken);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);

                    logger.LogInformation(
                        "[Serilog][{@LogType}]=>{@LogDesc}",
                        BehavioralLevel.ServiceType,
                        $"Scheduled posts processed. Found: {result.Found}, Published: {result.Published}, Failed: {result.Failed}");
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                    throw;
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Scheduled publish processing failed.");
                    await transaction.RollbackAsync(CancellationToken.None);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Scheduled publish worker iteration failed.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        logger.LogInformation("Scheduled publish worker stopped.");
    }
}
