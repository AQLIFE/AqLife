using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Shared.Exceptions;
using AqLife.Application.Abstractions.Commands;
using AqLife.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Validators;
using File;
using File.Service;

namespace File.Upload;

/// <summary>Starts a resumable video upload session.</summary>
public sealed record StartVideoUploadCommand(string FileName, long FileSize, Guid CategoryId, int ChunkSize = 4 * 1024 * 1024)
    : IRequest<ResumableUploadSession>, IApplicationRequest;

public sealed class StartVideoUploadValidator(IApplicationDbContext db)
    : AbstractValidator<StartVideoUploadCommand>
{
    protected override string ErrorMessage { init; get; } = "必须选择一个有效的文件分类";

    protected override async Task<bool> IsValidAsync(StartVideoUploadCommand command, CancellationToken ct)
        => command.CategoryId != Guid.Empty
            && await db.Categories.AnyAsync(category => category.UID == command.CategoryId, ct);
}

public sealed class StartVideoUploadHandler(IResumableFileUploadService uploads)
    : IRequestHandler<StartVideoUploadCommand, ResumableUploadSession>
{
    public Task<ResumableUploadSession> Handle(StartVideoUploadCommand request, CancellationToken ct)
        => uploads.StartAsync(new StartVideoUploadRequest(request.FileName, request.FileSize, request.CategoryId, request.ChunkSize), ct);
}

/// <summary>Reads the uploaded chunk list without mutating the upload session.</summary>
public sealed record GetVideoUploadStatusQuery(Guid UploadId) : IRequest<ResumableUploadStatus>;

public sealed class GetVideoUploadStatusHandler(IResumableFileUploadService uploads)
    : IRequestHandler<GetVideoUploadStatusQuery, ResumableUploadStatus>
{
    public Task<ResumableUploadStatus> Handle(GetVideoUploadStatusQuery request, CancellationToken ct)
        => uploads.GetStatusAsync(request.UploadId, ct);
}

/// <summary>Persists one video chunk.</summary>
public sealed record UploadVideoChunkCommand(Guid UploadId, int Index, IFormFile Chunk) : IRequest<Unit>;

public sealed class UploadVideoChunkHandler(IResumableFileUploadService uploads)
    : IRequestHandler<UploadVideoChunkCommand, Unit>
{
    public async Task<Unit> Handle(UploadVideoChunkCommand request, CancellationToken ct)
    {
        await uploads.WriteChunkAsync(request.UploadId, request.Index, request.Chunk, ct);
        return Unit.Value;
    }
}

/// <summary>Assembles all chunks and delegates persistence/deduplication to the existing file pipeline.</summary>
public sealed record CompleteVideoUploadCommand(Guid UploadId) : IRequest<Guid>;

public sealed class CompleteVideoUploadHandler(
    IResumableFileUploadService uploads,
    IApplicationDbContext db,
    ISender sender) : IRequestHandler<CompleteVideoUploadCommand, Guid>
{
    public async Task<Guid> Handle(CompleteVideoUploadCommand request, CancellationToken ct)
    {
        var assembled = await uploads.AssembleAsync(request.UploadId, ct);
        try
        {
            if (assembled.CategoryId == Guid.Empty)
                throw new RequestCheckException("该上传任务没有有效分类信息，请重新选择分类并上传。");
            if (!await db.Categories.AnyAsync(category => category.UID == assembled.CategoryId, ct))
                throw new RequestCheckException("上传期间所选分类已不存在，请重新选择分类并上传。");

            await using var stream = new FileStream(assembled.TempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var formFile = new FormFile(stream, 0, assembled.FileSize, "File", assembled.FileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = Path.GetExtension(assembled.FileName).ToLowerInvariant() == ".webm"
                    ? "video/webm"
                    : "video/mp4"
            };
            return await sender.Send(new CreateFileCommand(formFile, assembled.CategoryId, null, DateTimeOffset.UtcNow.AddDays(7)), ct);
        }
        finally
        {
            await uploads.DeleteSessionAsync(request.UploadId);
        }
    }
}

/// <summary>Checks whether a SHA-256 hash already belongs to a non-template file.</summary>
public sealed record CheckFileHashQuery(string Hash) : IRequest<FileHashCheckResult>;

public sealed record FileHashMatch(Guid UID, string FileName, string Extension, ulong FileSize, DateTimeOffset UploadTime);
public sealed record FileHashCheckResult(bool IsDuplicate, FileHashMatch? ExistingFile);

public sealed class CheckFileHashHandler(IApplicationDbContext db)
    : IRequestHandler<CheckFileHashQuery, FileHashCheckResult>
{
    public async Task<FileHashCheckResult> Handle(CheckFileHashQuery request, CancellationToken ct)
    {
        var hash = request.Hash?.Trim().ToLowerInvariant();
        if (hash is null || hash.Length != 64 || hash.Any(c => !Uri.IsHexDigit(c)))
            throw new ArgumentException("文件哈希必须是 64 位 SHA-256 十六进制字符串。", nameof(request.Hash));

        var match = await db.File.AsNoTracking()
            .Where(file => file.FileHash == hash && !file.IsTemplate)
            .OrderBy(file => file.UploadTime)
            .Select(file => new FileHashMatch(file.UID, file.FileName + file.Extension, file.Extension, file.FileSize, file.UploadTime))
            .FirstOrDefaultAsync(ct);

        return new FileHashCheckResult(match is not null, match);
    }
}
