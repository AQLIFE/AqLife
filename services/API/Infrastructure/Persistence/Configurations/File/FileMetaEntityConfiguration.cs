using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.File;

public sealed class FileMetaEntityConfiguration : IEntityTypeConfiguration<FileMetaEntity>
{
    public void Configure(EntityTypeBuilder<FileMetaEntity> builder)
    {
        builder.HasOne(file => file.Category)
            .WithMany(category => category.Files)
            .HasForeignKey(file => file.CategoryUID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(file => file.PublishMeta)
            .WithOne()
            .HasForeignKey<PublishMetaEntity>(meta => meta.UID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(file => file.InteractionMeta)
            .WithOne()
            .HasForeignKey<InteractionMetaEntity>(meta => meta.UID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}