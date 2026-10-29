using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Abstractions.FileStorage;

/// <summary>
/// Contract for temporary, disk-backed resumable uploads. Implementations belong to Infrastructure.
/// </summary>
public interface IResumableFileUploadService
{
    public sealed record StartRequest(string FileName, long FileSize, int ChunkSize = 4 * 1024 * 1024);
    public sealed record UploadSession(Guid UploadId, string FileName, long FileSize, int ChunkSize, int ChunkCount, DateTimeOffset CreatedAt);
    public sealed record UploadStatus(Guid UploadId, long FileSize, int ChunkSize, int ChunkCount, int[] UploadedChunks);
    public sealed record CompletedUpload(string FileName, long FileSize, string TempFilePath);

    Task<UploadSession> StartAsync(StartRequest request, CancellationToken ct);
    Task<UploadStatus> GetStatusAsync(Guid id, CancellationToken ct);
    Task WriteChunkAsync(Guid id, int index, IFormFile chunk, CancellationToken ct);
    Task<CompletedUpload> AssembleAsync(Guid id, CancellationToken ct);
    Task DeleteSessionAsync(Guid id);
}
