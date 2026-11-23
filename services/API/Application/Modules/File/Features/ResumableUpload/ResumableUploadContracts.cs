namespace AqLife.Application.File;
public sealed record StartVideoUploadRequest(string FileName, long FileSize, Guid CategoryId, int ChunkSize = 4 * 1024 * 1024);
public sealed record ResumableUploadSession(Guid UploadId, string FileName, long FileSize, Guid CategoryId, int ChunkSize, int ChunkCount, DateTimeOffset CreatedAt);
public sealed record ResumableUploadStatus(Guid UploadId, long FileSize, Guid CategoryId, int ChunkSize, int ChunkCount, int[] UploadedChunks);
public sealed record CompletedResumableUpload(string FileName, long FileSize, Guid CategoryId, string TempFilePath);
