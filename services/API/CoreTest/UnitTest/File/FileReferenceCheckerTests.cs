using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.File.Service;

namespace AqLife.CoreTest.UnitTest.File;

public sealed class FileReferenceCheckerTests
{
    [Fact]
    public async Task IsReferencedAsync_ReturnsTrue_WhenAnyModuleReportsReference()
    {
        var checker = new FileReferenceChecker(
        [
            new StubFileReferenceProvider(false),
            new StubFileReferenceProvider(true),
            new StubFileReferenceProvider(false)
        ]);

        var result = await checker.IsReferencedAsync(Guid.NewGuid());

        Assert.True(result);
    }

    [Fact]
    public async Task IsReferencedAsync_ReturnsFalse_WhenNoModuleReportsReference()
    {
        var checker = new FileReferenceChecker(
        [
            new StubFileReferenceProvider(false),
            new StubFileReferenceProvider(false)
        ]);

        var result = await checker.IsReferencedAsync(Guid.NewGuid());

        Assert.False(result);
    }

    private sealed class StubFileReferenceProvider(bool isReferenced) : IFileReferenceProvider
    {
        public Task<bool> IsReferencedAsync(
            Guid fileId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(isReferenced);
    }
}
