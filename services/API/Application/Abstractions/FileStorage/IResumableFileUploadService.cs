using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Abstractions.FileStorage;

/// <summary>
/// Contract for temporary, disk-backed resumable uploads. Implementations belong to Infrastructure.
/// </summary>
public sealed record StartVideoUploadRequest(string FileName, long FileSize, int ChunkSize = 4 * 1024 * 1024);
public sealed record ResumableUploadSession(Guid UploadId, string FileName, long FileSize, int ChunkSize, int ChunkCount, DateTimeOffset CreatedAt);
public sealed record ResumableUploadStatus(Guid UploadId, long FileSize, int ChunkSize, int ChunkCount, int[] UploadedChunks);
public sealed record CompletedResumableUpload(string FileName, long FileSize, string TempFilePath);

public interface IResumableFileUploadService
{
    Task<ResumableUploadSession> StartAsync(StartVideoUploadRequest request, CancellationToken ct);
    Task<ResumableUploadStatus> GetStatusAsync(Guid id, CancellationToken ct);
    Task WriteChunkAsync(Guid id, int index, IFormFile chunk, CancellationToken ct);
    Task<CompletedResumableUpload> AssembleAsync(Guid id, CancellationToken ct);
    Task DeleteSessionAsync(Guid id);
}
