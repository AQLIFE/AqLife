namespace AqLife.Application.Abstractions.Contracts.File.Storage;

/// <summary>
/// 文件存储层的范围读取结果。
/// Start / End 使用包含式字节偏移；调用方负责释放返回的 Stream。
/// </summary>
public sealed record FileStorageRangeReadResult(
    Stream Stream,
    long TotalLength,
    long Start,
    long End)
{
    public long ContentLength => End - Start + 1;
}
