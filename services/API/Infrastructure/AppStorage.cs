using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Domain.Entities.Favorite;
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
        public DbSet<DevelopmentPlanEntity> DevelopmentPlans { get; set; }
        public DbSet<TodoDependencyEntity> TodoDependencies { get; set; }
        public DbSet<RoadmapNodeLayoutEntity> RoadmapNodeLayouts { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<FileTagEntity> BlogTags { get; set; }
        public DbSet<PublishMetaEntity> FilePublishMetas { get; set; }
        public DbSet<InteractionMetaEntity> FileInteractionMetas { get; set; }
        public DbSet<SystemStateEntity> SystemStates { get; set; }
        public DbSet<FavoriteItemEntity> FavoriteItems { get; set; }
        public DbSet<FavoriteMediaEntity> FavoriteMedia { get; set; }
        public DbSet<FavoriteTagEntity> FavoriteTags { get; set; }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
        {
            var transaction = await Database.BeginTransactionAsync(ct);
            return new EfTransaction(transaction);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FavoriteMediaEntity>().HasKey(x => new { x.FavoriteId, x.FileId });
            modelBuilder.Entity<FavoriteTagEntity>().HasKey(x => new { x.FavoriteId, x.TagId });

            modelBuilder.Entity<FavoriteItemEntity>()
                .HasMany(x => x.Media).WithOne(x => x.Favorite)
                .HasForeignKey(x => x.FavoriteId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FavoriteItemEntity>()
                .HasMany(x => x.Tags).WithOne(x => x.Favorite)
                .HasForeignKey(x => x.FavoriteId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FavoriteMediaEntity>()
                .HasOne<FileMetaEntity>().WithMany()
                .HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FavoriteTagEntity>()
                .HasOne<TagEntity>().WithMany()
                .HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileTagEntity>()
                .HasKey(ft => new { ft.FileId, ft.TagId });
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(ft => ft.File)
                .WithMany(f => f.FileTags)
                .HasForeignKey(ft => ft.FileId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FileTagEntity>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FileTags)
                .HasForeignKey(ft => ft.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileMetaEntity>()
                .HasOne(f => f.PublishMeta)
                .WithOne()
                .HasForeignKey<PublishMetaEntity>(p => p.UID)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FileMetaEntity>()
                .HasOne(f => f.InteractionMeta)
                .WithOne()
                .HasForeignKey<InteractionMetaEntity>(p => p.UID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountEntity>()
                .HasMany(x => x.Subscriptions)
                .WithOne()
                .HasForeignKey(s => s.AID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TagEntity>()
                .HasMany(e => e.FileTags)
                .WithOne(ft => ft.Tag)
                .HasForeignKey(ft => ft.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed a stable default plan so legacy Todo creation continues to have
            // a valid plan owner while plan-aware application commands are introduced.
            modelBuilder.Entity<DevelopmentPlanEntity>().HasData(new
            {
                UID = DevelopmentPlanEntity.DefaultPlanId,
                Name = "默认开发计划",
                Description = (string?)null,
                IsArchived = false,
                CreatedAt = DateTimeOffset.UnixEpoch,
                UpdatedAt = DateTimeOffset.UnixEpoch,
                DependencyVersion = 0L
            });

            modelBuilder.Entity<DevelopmentPlanEntity>()
                .Property(x => x.DependencyVersion)
                .IsConcurrencyToken();

            modelBuilder.Entity<TodoEntity>()
                .Property(x => x.PlanId)
                .HasDefaultValue(DevelopmentPlanEntity.DefaultPlanId);

            modelBuilder.Entity<TodoEntity>()
                .HasOne<DevelopmentPlanEntity>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Parent-child is an organizational relationship, not a cascade-delete tree.
            modelBuilder.Entity<TodoEntity>()
                .HasMany(x => x.Children)
                .WithOne()
                .HasForeignKey(x => x.FTID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne<DevelopmentPlanEntity>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne<TodoEntity>()
                .WithMany()
                .HasForeignKey(x => x.PredecessorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne<TodoEntity>()
                .WithMany()
                .HasForeignKey(x => x.SuccessorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoadmapNodeLayoutEntity>()
                .HasOne<DevelopmentPlanEntity>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoadmapNodeLayoutEntity>()
                .HasOne<TodoEntity>()
                .WithMany()
                .HasForeignKey(x => x.TodoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
