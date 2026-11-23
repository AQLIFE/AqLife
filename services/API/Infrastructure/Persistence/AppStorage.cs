using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure.Persistence
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppStorage).Assembly);
        }
    }
}
