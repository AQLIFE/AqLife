using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure
{
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<FileMetaEntity> File { get; set; }
        public DbSet<CorpusEntity> Corpus { get; set; }
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<SubscriptionEntity> Subscription { get; set; }
        public DbSet<TodoEntity> Todo { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<FileTagEntity> BlogTags { get; set; }
        public DbSet<PublishMetaEntity> FilePublishMetas { get; set; }
        public DbSet<InteractionMetaEntity> FileInteractionMetas { get; set; }


        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
        {
            var transaction =
           await Database.BeginTransactionAsync(ct);

            return new EfTransaction(transaction);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FileTag 复合主键
            modelBuilder.Entity<FileTagEntity>()
                .HasKey(ft => new { ft.FileId, ft.TagId });

            // FileMeta 1 : N FileTag
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(ft => ft.File)
                .WithMany(f => f.FileTags)
                .HasForeignKey(ft => ft.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tag 1 : N FileTag
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FileTags)
                .HasForeignKey(ft => ft.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // FileMeta 1 : 1 PublishMeta
            // PublishMeta.UID 同时作为 PK 和 FK
            modelBuilder.Entity<FileMetaEntity>()
                .HasOne(f => f.PublishMeta)
                .WithOne()
                .HasForeignKey<PublishMetaEntity>(p => p.UID)
                .OnDelete(DeleteBehavior.Cascade);

            // FileMeta 1 : 1 InteractionMeta
            // InteractionMeta.UID 同时作为 PK 和 FK
            modelBuilder.Entity<FileMetaEntity>()
                .HasOne(f => f.InteractionMeta)
                .WithOne()
                .HasForeignKey<InteractionMetaEntity>(p => p.UID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
