namespace AqLife.Application.Abstractions.Contracts.File;

/// <summary>
/// 由拥有文件引用的业务模块实现。
/// File 模块通过该契约检查文件是否仍被其他模块使用，避免直接依赖其他模块的数据查询实现。
/// </summary>
public interface IFileReferenceProvider
{
    /// <summary>
    /// 判断指定文件是否仍被当前业务模块引用。
    /// </summary>
    Task<bool> IsReferencedAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);
}
