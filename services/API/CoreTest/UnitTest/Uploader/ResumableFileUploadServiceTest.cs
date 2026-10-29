using AqLife.Shared.Options;
using AqLife.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AqLife.CoreTest.UnitTest.Uploader;

public sealed class ResumableFileUploadServiceTest : IDisposable
{
    private const int MinChunkSize = 256 * 1024;
    private readonly string _root = Path.Combine(Path.GetTempPath(), "AqLife-ResumableUploadTests", Guid.NewGuid().ToString("N"));
    private readonly ResumableFileUploadService _service;

    public ResumableFileUploadServiceTest()
    {
        Directory.CreateDirectory(_root);
        _service = new ResumableFileUploadService(Options.Create(new FilePolicyOption
        {
            StoragePath = _root,
            StorageUnit = 0,
            MaxFileSize = 1024,
            VideoMaxFileSize = 1024 * 1024,
            AllowedVideoExtensions = [".mp4", ".webm"],
            AllowedUpload = [".mp4", ".webm"]
        }));
    }

    [Fact]
    public async Task StartAsync_ShouldRejectNonVideoExtension()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.StartAsync(new("notes.md", 100, MinChunkSize), CancellationToken.None));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1_048_577)]
    public async Task StartAsync_ShouldRejectInvalidOrOversizedFile(long size)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _service.StartAsync(new("movie.mp4", size, MinChunkSize), CancellationToken.None));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9 * 1024 * 1024)]
    public async Task StartAsync_ShouldRejectInvalidChunkSize(int chunkSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _service.StartAsync(new("movie.mp4", 100, chunkSize), CancellationToken.None));
    }

    [Fact]
    public async Task UploadStatusAndAssemble_ShouldSupportOutOfOrderChunksAndResume()
    {
        var bytes = Enumerable.Range(0, MinChunkSize * 2 + 17).Select(i => (byte)(i % 251)).ToArray();
        var session = await _service.StartAsync(new("movie.mp4", bytes.Length, MinChunkSize), CancellationToken.None);

        await using (var last = new MemoryStream(bytes[(MinChunkSize * 2)..]))
            await _service.WriteChunkAsync(session.UploadId, 2, ToFormFile(last, "last"), CancellationToken.None);
        await using (var first = new MemoryStream(bytes[..MinChunkSize]))
            await _service.WriteChunkAsync(session.UploadId, 0, ToFormFile(first, "first"), CancellationToken.None);

        var status = await _service.GetStatusAsync(session.UploadId, CancellationToken.None);
        Assert.Equal(new[] { 0, 2 }, status.UploadedChunks);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AssembleAsync(session.UploadId, CancellationToken.None));

        await using (var middle = new MemoryStream(bytes[MinChunkSize..(MinChunkSize * 2)]))
            await _service.WriteChunkAsync(session.UploadId, 1, ToFormFile(middle, "middle"), CancellationToken.None);

        var assembled = await _service.AssembleAsync(session.UploadId, CancellationToken.None);
        try
        {
            Assert.Equal(bytes.Length, assembled.FileSize);
            Assert.Equal("movie.mp4", assembled.FileName);
            Assert.Equal(bytes, await File.ReadAllBytesAsync(assembled.TempFilePath, TestContext.Current.CancellationToken));
        }
        finally
        {
            await _service.DeleteSessionAsync(session.UploadId);
        }

        Assert.Empty(Directory.EnumerateFileSystemEntries(Path.Combine(_root, ".uploads")));
    }

    [Fact]
    public async Task WriteChunkAsync_ShouldRejectWrongLengthAndDuplicateChunk()
    {
        var session = await _service.StartAsync(new("movie.webm", MinChunkSize, MinChunkSize), CancellationToken.None);
        await using var wrong = new MemoryStream([1, 2, 3]);
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.WriteChunkAsync(session.UploadId, 0, ToFormFile(wrong, "wrong"), CancellationToken.None));

        var bytes = new byte[MinChunkSize];
        await using var valid = new MemoryStream(bytes);
        await _service.WriteChunkAsync(session.UploadId, 0, ToFormFile(valid, "valid"), CancellationToken.None);

        await using var duplicate = new MemoryStream(bytes);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.WriteChunkAsync(session.UploadId, 0, ToFormFile(duplicate, "duplicate"), CancellationToken.None));

        await _service.DeleteSessionAsync(session.UploadId);
    }

    [Fact]
    public void FilePolicyOption_ShouldCalculateLimitsInLongBytes()
    {
        var policy = new FilePolicyOption
        {
            MaxFileSize = 100,
            VideoMaxFileSize = 1_048_576,
            StorageUnit = 10
        };

        Assert.Equal(100L * 1024, policy.DefaultFileSize);
        Assert.Equal(1_073_741_824L, policy.VideoFileSize);
    }

    private static IFormFile ToFormFile(Stream stream, string name)
        => new FormFile(stream, 0, stream.Length, "chunk", name)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }
}
