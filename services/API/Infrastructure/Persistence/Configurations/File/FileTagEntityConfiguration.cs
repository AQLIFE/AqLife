using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.File;

public sealed class FileTagEntityConfiguration : IEntityTypeConfiguration<FileTagEntity>
{
    public void Configure(EntityTypeBuilder<FileTagEntity> builder)
    {
        builder.HasKey(fileTag => new { fileTag.FileId, fileTag.TagId });

        builder.HasOne(fileTag => fileTag.File)
            .WithMany(file => file.FileTags)
            .HasForeignKey(fileTag => fileTag.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fileTag => fileTag.Tag)
            .WithMany(tag => tag.FileTags)
            .HasForeignKey(fileTag => fileTag.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}