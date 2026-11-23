using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Favorite;

public sealed class FavoriteMediaEntityConfiguration : IEntityTypeConfiguration<FavoriteMediaEntity>
{
    public void Configure(EntityTypeBuilder<FavoriteMediaEntity> builder)
    {
        builder.HasKey(media => new { media.FavoriteId, media.FileId });

        builder.HasOne<FileMetaEntity>()
            .WithMany()
            .HasForeignKey(media => media.FileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}