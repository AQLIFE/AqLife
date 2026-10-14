using AqLife.Application.Business.File.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Extensions.Database;
using AqLife.Infrastructure;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace AqLife.CoreTest.IntegrationTest.File;

[Collection<IntegrationTestCollection>]
public class FileQueryTests(IntegrationTestFixture fixture) : IntegrationTestBase<FileMetaEntity>(fixture)
{
    [Fact, ResetDatabase]
    public async Task AnonymousQueries_ShouldReturnOnlyPublishedNonTemplateFilesWithAllowedExtensions()
    {
        SetAnonymousUser();
        var files = await SeedFilesAsync();

        var images = await SearchAsync(new FileQuery(Scope: FileScope.Image, Order: FileOrder.MostViewed));
        var blogs = await SearchAsync(new FileQuery(Scope: FileScope.Blog, Order: FileOrder.MostViewed));
        var defaultFiles = await SearchAsync(new FileQuery(Scope: FileScope.Default, Order: FileOrder.MostViewed));
        var allFiles = await SearchAsync(new FileQuery(Scope: FileScope.All, Order: FileOrder.MostViewed));

        Assert.Equal(new[] { files.PublishedImage }, images.Items.Select(item => item.UID));
        Assert.Equal(new[] { files.PublishedBlog }, blogs.Items.Select(item => item.UID));

        var expectedPublicFiles = new[] { files.PublishedImage, files.PublishedBlog }.Order();
        Assert.Equal(expectedPublicFiles, defaultFiles.Items.Select(item => item.UID).Order());
        Assert.Equal(expectedPublicFiles, allFiles.Items.Select(item => item.UID).Order());

        Assert.DoesNotContain(defaultFiles.Items, item => item.IsTemplate);
        Assert.All(defaultFiles.Items, item => Assert.Equal(FileStatus.Published, item.PublishMeta.PublishStatus));
        Assert.DoesNotContain(defaultFiles.Items, item => item.Extension == ".exe");
    }

    [Theory, ResetDatabase]
    [InlineData(FileScope.Template)]
    [InlineData(FileScope.All)]
    public async Task AnonymousTemplateOrAllScope_ShouldThrowForbiddenException(FileScope scope)
    {
        SetAnonymousUser();
        await SeedFilesAsync();

        var exception = await Assert.ThrowsAsync<ForbiddenException>(
            () => ExecuteAsync(
                new FileQuery(Scope: scope, Order: FileOrder.MostViewed),
                TestContext.Current.CancellationToken));

        Assert.Equal("无权搜索", exception.Message);
    }

    [Fact, ResetDatabase]
    public async Task AuthenticatedAllScope_ShouldIncludeTemplatesAndUnpublishedFiles()
    {
        var files = await SeedFilesAsync();
        SetAuthenticatedUser();

        var result = await SearchAsync(new FileQuery(Scope: FileScope.All, Order: FileOrder.MostViewed));

        var resultIds = result.Items.Select(item => item.UID).ToHashSet();
        Assert.Contains(files.PublishedImage, resultIds);
        Assert.Contains(files.DraftImage, resultIds);
        Assert.Contains(files.PublishedBlog, resultIds);
        Assert.Contains(files.Template, resultIds);
        Assert.Contains(result.Items, item => item.IsTemplate);
        Assert.Contains(result.Items, item => item.PublishMeta.PublishStatus == FileStatus.Draft);
    }

    [Fact, ResetDatabase]
    public async Task UnknownCategory_ShouldThrowRequestCheckException()
    {
        var exception = await Assert.ThrowsAsync<RequestCheckException>(
            () => ExecuteAsync(
                new FileQuery(CategoryUID: Guid.NewGuid(), Order: FileOrder.MostViewed),
                TestContext.Current.CancellationToken));

        Assert.Equal("无效的搜索条件", exception.Message);
    }

    private async Task<PageResult<FileMetaEntity>> SearchAsync(FileQuery query)
    {
        using var scope = CreateScope();
        var search = scope.ServiceProvider.GetRequiredService<FileSearch>();
        return await search.SearchPageAsync(query, TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// 在数据库中构造一组虚拟文件(图像*1,普通博文*1,模板*1,以及不允许的扩展名文件*1)
    /// </summary>
    /// <returns></returns>
    private async Task<(Guid PublishedImage, Guid DraftImage, Guid PublishedBlog, Guid Template)> SeedFilesAsync()
    {
        var publishedImage = CreateFile("published.png");
        publishedImage.PublishMeta.Publish();// 发布图像*1
        var draftImage = CreateFile("draft.png");// 未发布图像*1
        var publishedBlog = CreateFile("published.md");
        publishedBlog.PublishMeta.Publish();//发布博文*1
        var template = CreateFile("template.md").SetTemplate();//模板*1
        var unsupportedFile = CreateFile("unsupported.exe");// 不允许的扩展名文件*1

        using var scope = CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();
        storage.File.AddRange(publishedImage, draftImage, publishedBlog, template, unsupportedFile);
        await storage.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (publishedImage.UID, draftImage.UID, publishedBlog.UID, template.UID);
    }

    private static FileMetaEntity CreateFile(string fileName)
    {
        using var stream = new MemoryStream([1]);
        var file = new FormFile(stream, 0, stream.Length, "file", fileName);
        return new FileMetaEntity(file, $"hash-{fileName}");
    }
}
