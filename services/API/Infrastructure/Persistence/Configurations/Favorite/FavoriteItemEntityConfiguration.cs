using AqLife.Domain.Entities;
using AqLife.Domain.Entities.Favorite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Favorite;

public sealed class FavoriteItemEntityConfiguration : IEntityTypeConfiguration<FavoriteItemEntity>
{
    public void Configure(EntityTypeBuilder<FavoriteItemEntity> builder)
    {
        builder.Property(item => item.CategoryUID)
            .HasDefaultValue(CategoryEntity.DefaultCategoryId);

        builder.HasMany(item => item.Media)
            .WithOne()
            .HasForeignKey(media => media.FavoriteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Category)
            .WithMany(category => category.Favorites)
            .HasForeignKey(item => item.CategoryUID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(item => item.Tags)
            .WithOne()
            .HasForeignKey(tag => tag.FavoriteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}