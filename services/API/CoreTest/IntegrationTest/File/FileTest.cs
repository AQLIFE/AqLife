using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AqLife.CoreTest.IntegrationTest.File;

[Collection<IntegrationTestCollection>]
public class FileTest(IntegrationTestFixture fixture) : IntegrationTestBase<FileMetaEntity>(fixture)
{
    [Fact, ResetDatabase]
    public async Task DeleteAsset_ShouldKeepFile_WhenFavoriteModuleStillReferencesIt()
    {
        using var scope = CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<AppStorage>();
        var fileAssetService = scope.ServiceProvider.GetRequiredService<IFileAssetService>();

        using var stream = new MemoryStream([1, 2, 3]);
        var formFile = new FormFile(stream, 0, stream.Length, "file", "favorite-reference.png");
        var file = new FileMetaEntity(formFile, CategoryEntity.DefaultCategoryId);
        var favorite = new FavoriteItemEntity(
            "File reference guard test",
            string.Empty,
            CategoryEntity.DefaultCategoryId);
        favorite.Media.Add(new FavoriteMediaEntity(file.UID, favorite.UID));

        storage.File.Add(file);
        storage.FavoriteItems.Add(favorite);
        await storage.SaveChangesAsync();

        try
        {
            await fileAssetService.DeleteAsync([file.UID]);

            Assert.True(await storage.File.AnyAsync(entity => entity.UID == file.UID));
        }
        finally
        {
            storage.FavoriteItems.Remove(favorite);
            storage.File.Remove(file);
            await storage.SaveChangesAsync();
        }
    }
}
