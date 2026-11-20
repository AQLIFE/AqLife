using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AqLife.Infrastructure.BackServices
{
    /// <summary>
    /// 处理所有已到期的预约发布,将其发布到指定位置,并返回处理结果:后台任务
    /// </summary>
    public record ProcessScheduledPostsCommand : IRequest<ProcessScheduledPostsResult>;// 需要手动注册相关依赖
    public class ProcessScheduledPostsResult(
    int Found,
    int Published,
    int Failed)
    {
        public int Found { get; set; } = Found;
        public int Published { get; set; } = Published;
        public int Failed { get; set; } = Failed;
    };

    public class ScheduledPublishWorker(IServiceScopeFactory scopeFactory, ILogger<ScheduledPublishWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ServiceType, "Scheduled publish worker started.");
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
                        logger.LogInformation("[Serilog][{@LogType}]=>{@LogDesc}", BehavioralLevel.ServiceType, $"Scheduled posts processed. Found: {result.Found}, Published: {result.Published}, Failed: {result.Failed}");
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

