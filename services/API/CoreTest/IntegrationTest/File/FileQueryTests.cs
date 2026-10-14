using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Extensions.Database;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AqLife.CoreTest.IntegrationTest.File;

[Collection<IntegrationTestCollection>]
public class FileQueryTests(IntegrationTestFixture fixture) : FileTest(fixture)
{
    [Fact, ResetDatabase]
    public async Task AnonymousQueries_ShouldReturnOnlyPublishedNonTemplateFiles()
    {
        var files = await SeedFilesAsync();

        var images = await ExecuteAsync(new FileQuery(Scope: FileScope.Image, Order: FileOrder.MostViewed));
        var blogs = await ExecuteAsync(new FileQuery(Scope: FileScope.Blog, Order: FileOrder.MostViewed));
        var allPublicFiles = await ExecuteAsync(new FileQuery(Scope: FileScope.Default, Order: FileOrder.MostViewed));

        Assert.Equal([files.PublishedImage], images.Items.Select(item => item.UID));
        Assert.Equal([files.PublishedBlog], blogs.Items.Select(item => item.UID));
        Assert.Equal(
            new[] { files.PublishedImage, files.PublishedBlog }.Order(),
            allPublicFiles.Items.Select(item => item.UID).Order());
        Assert.DoesNotContain(allPublicFiles.Items, item => item.IsTemplate);
        Assert.All(allPublicFiles.Items, item => Assert.Equal(FileStatus.Published, item.PublishStatus));
    }

    [Theory, ResetDatabase]
    [InlineData(FileScope.Template)]
    [InlineData(FileScope.All)]
    public async Task AnonymousTemplateOrAllScope_ShouldThrowForbiddenException(FileScope scope)
    {
        await SeedFilesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => ExecuteAsync(new FileQuery(Scope: scope, Order: FileOrder.MostViewed)));

        Assert.Equal("无权搜索", exception.Message);
    }

    [Fact, ResetDatabase]
    public async Task AuthenticatedAllScope_ShouldIncludeTemplatesAndUnpublishedFiles()
    {
        var files = await SeedFilesAsync();
        SetAuthenticatedUser();

        var result = await ExecuteAsync(new FileQuery(Scope: FileScope.All, Order: FileOrder.MostViewed));

        var resultIds = result.Items.Select(item => item.UID).ToHashSet();
        Assert.Contains(files.PublishedImage, resultIds);
        Assert.Contains(files.DraftImage, resultIds);
        Assert.Contains(files.PublishedBlog, resultIds);
        Assert.Contains(files.Template, resultIds);
        Assert.Contains(result.Items, item => item.IsTemplate);
        Assert.Contains(result.Items, item => item.PublishStatus == FileStatus.Draft);
    }

    [Fact, ResetDatabase]
    public async Task UnknownCategory_ShouldThrowRequestCheckException()
    {
        var exception = await Assert.ThrowsAsync<RequestCheckException>(
            () => ExecuteAsync(new FileQuery(CategoryUID: Guid.NewGuid())));

        Assert.Equal("无效的搜索条件", exception.Message);
    }

    private void SetAuthenticatedUser()
    {
        var accessor = fixture.Services.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext ??= new DefaultHttpContext();
        accessor.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "test-user")], "test"));
    }

    private async Task<(Guid PublishedImage, Guid DraftImage, Guid PublishedBlog, Guid Template)> SeedFilesAsync()
    {
        var publishedImage = CreateFile("published.png");
        publishedImage.PublishMeta.Publish();
        var draftImage = CreateFile("draft.png");
        var publishedBlog = CreateFile("published.md");
        publishedBlog.PublishMeta.Publish();
        var template = CreateFile("template.md").SetTemplate();

        using var scope = CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();
        storage.File.AddRange(publishedImage, draftImage, publishedBlog, template);
        await storage.SaveChangesAsync(CancellationToken.None);

        return (publishedImage.UID, draftImage.UID, publishedBlog.UID, template.UID);
    }

    private static FileMetaEntity CreateFile(string fileName)
    {
        var stream = new MemoryStream([1]);
        var file = new FormFile(stream, 0, stream.Length, "file", fileName);
        return new FileMetaEntity(file, $"hash-{fileName}");
    }
}
