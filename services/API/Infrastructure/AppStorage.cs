using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
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
        public DbSet<CategoryEntity> Categories { get; set; }

        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
        {
            var transaction = await Database.BeginTransactionAsync(ct);
            return new EfTransaction(transaction);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FavoriteMediaEntity>().HasKey(x => new { x.FavoriteId, x.FileId });
            //modelBuilder.Entity<FavoriteMediaEntity>()
            //    .HasIndex(x => new { x.FavoriteId, x.FileId })
            //    .IsUnique();
            modelBuilder.Entity<FavoriteTagEntity>().HasKey(x => new { x.FavoriteId, x.TagId });

            modelBuilder.Entity<FavoriteItemEntity>()
                .HasMany(x => x.Media).WithOne()
                .HasForeignKey(x => x.FavoriteId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FavoriteItemEntity>()
                .HasOne(x => x.Category).WithMany(e=>e.Favorites)
                .HasForeignKey(x => x.CategoryUID)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<FavoriteItemEntity>()
                .HasMany(x => x.Tags).WithOne()
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

            modelBuilder.Entity<CategoryEntity>().HasData(new CategoryEntity("默认分类", "一个基础分类,不允许删除", null)
            {
                UID = CategoryEntity.DefaultCategoryId
            });
            // Seed the stable plan that owns rows migrated from the legacy single-plan Todo model.
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
                .HasOne<DevelopmentPlanEntity>(e=>e.Plan)
                .WithMany(t=>t.Todos)
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TodoEntity>()
                .HasOne(e => e.LayoutEntity)
                .WithOne()
                .HasForeignKey<RoadmapNodeLayoutEntity>(e => e.UID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne<DevelopmentPlanEntity>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne(e => e.PredecessorTodo)
                .WithMany(e => e.OutgoingDependencies)
                .HasForeignKey(e => e.PredecessorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TodoDependencyEntity>()
                .HasOne(e => e.SuccessorTodo)
                .WithMany(e => e.IncomingDependencies)
                .HasForeignKey(e => e.SuccessorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoadmapNodeLayoutEntity>()
                .HasOne<DevelopmentPlanEntity>()
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoadmapNodeLayoutEntity>()
                .HasKey(e => e.UID);
            //modelBuilder.Entity<RoadmapNodeLayoutEntity>()
            //    .HasOne<TodoEntity>()
            //    .WithMany()
            //    .HasForeignKey(x => x.UID)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
