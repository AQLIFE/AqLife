namespace AqLife.Application.Abstractions.Contracts.File.Storage;

/// <summary>
/// 物理文件存储端口。实现可以是本地文件系统或对象存储。
/// </summary>
public interface IFileStorage
{
    Task SaveAsync(Stream content, string key, CancellationToken ct = default);

    Task<bool> DeleteAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// 打开完整文件流；调用方负责释放返回的 Stream。
    /// </summary>
    Task<Stream> OpenReadAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// 打开指定字节范围。start 与 end 均为包含式偏移；调用方负责释放结果中的 Stream。
    /// </summary>
    Task<FileStorageRangeReadResult> OpenReadRangeAsync(
        string key,
        long start,
        long end,
        CancellationToken ct = default);
}
