namespace AqLife.Application.Abstractions.Contracts.File;

/// <summary>
/// 定时发布应用用例的执行入口，由 Application 模块实现，供 Infrastructure Worker 调用。
/// </summary>
public interface IScheduledPublishProcessor
{
    Task<ScheduledPublishResult> ProcessDueAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 一轮定时发布的处理统计。
/// </summary>
public sealed record ScheduledPublishResult(int Found, int Published, int Failed);
