using AqLife.Domain.Entities;
using AqLife.Domain.Entities.Favorite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Favorite;

public sealed class FavoriteTagEntityConfiguration : IEntityTypeConfiguration<FavoriteTagEntity>
{
    public void Configure(EntityTypeBuilder<FavoriteTagEntity> builder)
    {
        builder.HasKey(tag => new { tag.FavoriteId, tag.TagId });

        builder.HasOne<TagEntity>()
            .WithMany()
            .HasForeignKey(tag => tag.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}