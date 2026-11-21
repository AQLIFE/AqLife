namespace AqLife.Application.Abstractions.Contracts.File;

/// <summary>
/// Infrastructure 提供的定时文件查询端口。
/// 只返回待处理文件标识，不向应用层泄漏 EF Core 查询对象或存储实现。
/// </summary>
public interface IScheduledFileQuery
{
    Task<IReadOnlyList<Guid>> GetDueFileIdsAsync(
        DateTimeOffset asOf,
        CancellationToken cancellationToken = default);
}
