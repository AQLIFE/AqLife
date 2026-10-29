using System.Text.Json;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AqLife.Web.Services;

/// <summary>
/// Disk-backed resumable upload sessions. For multi-instance deployments, the configured storage path must be shared.
/// </summary>
public sealed class ResumableFileUploadService(IOptions<FilePolicyOption> options)
{
    private readonly FilePolicyOption policy = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, SemaphoreSlim> Locks = new();

    private string RootPath => Path.Combine(Path.GetFullPath(policy.StoragePath), ".uploads");
    private string SessionPath(Guid id) => Path.Combine(RootPath, id.ToString("N"));
    private string MetadataPath(Guid id) => Path.Combine(SessionPath(id), "session.json");
    private string ChunkPath(Guid id, int index) => Path.Combine(SessionPath(id), $"chunk-{index:D8}.part");

    public sealed record StartRequest(string FileName, long FileSize, int ChunkSize = 4 * 1024 * 1024);
    public sealed record UploadSession(Guid UploadId, string FileName, long FileSize, int ChunkSize, int ChunkCount, DateTimeOffset CreatedAt);
    public sealed record UploadStatus(Guid UploadId, long FileSize, int ChunkSize, int ChunkCount, int[] UploadedChunks);
    public sealed record CompletedUpload(string FileName, long FileSize, string TempFilePath);

    public async Task<UploadSession> StartAsync(StartRequest request, CancellationToken ct)
    {
        var name = Path.GetFileName(request.FileName);
        var extension = Path.GetExtension(name).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(name) || !policy.AllowedVideoExtensions.Contains(extension))
            throw new ArgumentException("仅允许创建视频文件的断点上传任务。");
        var limit = GetVideoLimitBytes();
        if (request.FileSize <= 0 || limit <= 0 || request.FileSize > limit)
            throw new ArgumentOutOfRangeException(nameof(request.FileSize), $"视频大小不得超过 {limit} 字节。");
        if (request.ChunkSize < 256 * 1024 || request.ChunkSize > 8 * 1024 * 1024)
            throw new ArgumentOutOfRangeException(nameof(request.ChunkSize), "分片大小必须在 256 KiB 到 8 MiB 之间。");

        Directory.CreateDirectory(RootPath);
        var id = Guid.NewGuid();
        var session = new UploadSession(id, name, request.FileSize, request.ChunkSize,
            checked((int)((request.FileSize + request.ChunkSize - 1) / request.ChunkSize)), DateTimeOffset.UtcNow);
        Directory.CreateDirectory(SessionPath(id));
        await File.WriteAllTextAsync(MetadataPath(id), JsonSerializer.Serialize(session, JsonOptions), ct);
        return session;
    }

    public async Task<UploadStatus> GetStatusAsync(Guid id, CancellationToken ct)
    {
        var session = await ReadSessionAsync(id, ct);
        var uploaded = Enumerable.Range(0, session.ChunkCount).Where(i => File.Exists(ChunkPath(id, i))).ToArray();
        return new UploadStatus(id, session.FileSize, session.ChunkSize, session.ChunkCount, uploaded);
    }

    public async Task WriteChunkAsync(Guid id, int index, IFormFile chunk, CancellationToken ct)
    {
        var session = await ReadSessionAsync(id, ct);
        if (index < 0 || index >= session.ChunkCount)
            throw new ArgumentOutOfRangeException(nameof(index), "分片序号无效。");
        var expectedLength = index == session.ChunkCount - 1
            ? session.FileSize - (long)session.ChunkSize * index
            : session.ChunkSize;
        if (chunk is null || chunk.Length != expectedLength)
            throw new ArgumentException($"分片长度不匹配，期望 {expectedLength} 字节。");

        var gate = Locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(ct);
        try
        {
            if (File.Exists(ChunkPath(id, index)))
                throw new InvalidOperationException("该分片已上传；请先查询任务状态，不允许覆盖已确认的分片。");
            await using var output = new FileStream(ChunkPath(id, index), FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await using var input = chunk.OpenReadStream();
            await input.CopyToAsync(output, ct);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<CompletedUpload> AssembleAsync(Guid id, CancellationToken ct)
    {
        var session = await ReadSessionAsync(id, ct);
        var gate = Locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(ct);
        try
        {
            for (var i = 0; i < session.ChunkCount; i++)
                if (!File.Exists(ChunkPath(id, i)))
                    throw new InvalidOperationException($"缺少分片 {i}，无法完成上传。");

            var assembledPath = Path.Combine(SessionPath(id), "assembled.tmp");
            if (File.Exists(assembledPath))
                File.Delete(assembledPath);
            await using (var output = new FileStream(assembledPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                for (var i = 0; i < session.ChunkCount; i++)
                {
                    await using var input = new FileStream(ChunkPath(id, i), FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
                    await input.CopyToAsync(output, ct);
                }
            }

            var info = new FileInfo(assembledPath);
            if (info.Length != session.FileSize)
            {
                File.Delete(assembledPath);
                throw new InvalidOperationException("合并后文件大小与声明大小不一致。");
            }
            return new CompletedUpload(session.FileName, session.FileSize, assembledPath);
        }
        finally
        {
            gate.Release();
        }
    }

    public Task DeleteSessionAsync(Guid id)
    {
        var path = SessionPath(id);
        if (Directory.Exists(path))
            Directory.Delete(path, recursive: true);
        if (Locks.TryRemove(id, out var gate))
            gate.Dispose();
        return Task.CompletedTask;
    }

    private async Task<UploadSession> ReadSessionAsync(Guid id, CancellationToken ct)
    {
        var path = MetadataPath(id);
        if (!File.Exists(path))
            throw new FileNotFoundException("上传任务不存在或已过期。");
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<UploadSession>(stream, JsonOptions, ct)
            ?? throw new InvalidDataException("上传任务元数据损坏。");
    }

    private long GetVideoLimitBytes()
    {
        if (policy.VideoMaxFileSize <= 0 || policy.StorageUnit < 0 || policy.StorageUnit > 50)
            return 0;
        return (long)policy.VideoMaxFileSize << policy.StorageUnit;
    }
}