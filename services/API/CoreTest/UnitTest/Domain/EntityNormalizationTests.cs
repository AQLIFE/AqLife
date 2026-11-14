using AqLife.Domain.Entities;
using AqLife.Domain.Entities.Favorite;

namespace AqLife.CoreTest.UnitTest.Domain;

public class EntityNormalizationTests
{
    [Fact]
    public void AccountAndSubscriptionConstructors_TrimUserFacingFields()
    {
        var account = new AccountEntity("  Account  ", "  Description  ", " Password ");

        Assert.Equal("Account", account.Name);
        Assert.Equal("Description", account.Desc);

        var subscription = new SubscriptionEntity(
            Guid.NewGuid(),
            "  https://example.com/feed  ",
            "  Platform  ",
            "  Alias  ",
            Guid.NewGuid());

        Assert.Equal("https://example.com/feed", subscription.SubscriptionLink);
        Assert.Equal("Platform", subscription.SubscriptionPlatform);
        Assert.Equal("Alias", subscription.AliasName);
    }

    [Fact]
    public void CategoryConstructor_TrimsNameAndDescription()
    {
        var category = new CategoryEntity("  Category  ", "  Description  ");

        Assert.Equal("Category", category.CategoryName);
        Assert.Equal("Description", category.CategoryDescription);
    }

    [Fact]
    public void FavoriteItemConstructor_TrimsTitleAndUrlButPreservesDescriptionContent()
    {
        var favorite = new FavoriteItemEntity(
            "  Favorite title  ",
            "  Markdown content  ",
            CategoryEntity.DefaultCategoryId,
            "  https://example.com/article  ");

        Assert.Equal("Favorite title", favorite.Title);
        Assert.Equal("  Markdown content  ", favorite.Description);
        Assert.Equal("https://example.com/article", favorite.SourceUrl);
    }

    [Fact]
    public void FavoriteMediaConstructor_TrimsRole()
    {
        var media = new FavoriteMediaEntity(Guid.NewGuid(), Guid.NewGuid(), "  Cover  ");
        Assert.Equal("Cover", media.Role);
    }

    [Fact]
    public void TagConstructor_TrimsNameAndAlias()
    {
        var tag = new TagEntity("  .NET  ", "  Dotnet  ");
        Assert.Equal(".NET", tag.Name);
        Assert.Equal("Dotnet", tag.AliasName);
    }
}
