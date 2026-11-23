using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Persistence.Queries;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure.Persistence
{
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options), IApplicationDbContext, IModuleQuerySources, IAccountPersistence, ICategoryPersistence
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

        IQueryable<AccountEntity> IAccountQuerySource.Query() => Accounts;
        IQueryable<AccountEntity> IAccountPersistence.Accounts => Accounts;
        IQueryable<SystemStateEntity> IAccountPersistence.SystemStates => SystemStates;
        async Task IAccountPersistence.AddAccountAsync(AccountEntity account, CancellationToken cancellationToken)
            => await Accounts.AddAsync(account, cancellationToken);
        void IAccountPersistence.RemoveAccount(AccountEntity account) => Accounts.Remove(account);
        async Task IAccountPersistence.AddSystemStateAsync(SystemStateEntity state, CancellationToken cancellationToken)
            => await SystemStates.AddAsync(state, cancellationToken);
        void IAccountPersistence.AddSubscriptions(IEnumerable<SubscriptionEntity> subscriptions)
            => Subscription.AddRange(subscriptions);
        void IAccountPersistence.RemoveSubscriptions(IEnumerable<SubscriptionEntity> subscriptions)
            => Subscription.RemoveRange(subscriptions);
        IQueryable<CategoryEntity> ICategoryQuerySource.Query() => Categories;
        IQueryable<CategoryEntity> ICategoryPersistence.Categories => Categories;
        async Task ICategoryPersistence.AddCategoryAsync(CategoryEntity category, CancellationToken cancellationToken)
            => await Categories.AddAsync(category, cancellationToken);
        void ICategoryPersistence.RemoveCategory(CategoryEntity category) => Categories.Remove(category);
        IQueryable<CorpusEntity> ICorpusQuerySource.Query() => Corpus;
        IQueryable<FavoriteItemEntity> IFavoriteItemQuerySource.Query() => FavoriteItems;
        IQueryable<FileMetaEntity> IFileMetaQuerySource.Query() => File;
        IQueryable<DevelopmentPlanEntity> IDevelopmentPlanQuerySource.Query() => DevelopmentPlans;
        IQueryable<TodoEntity> ITodoQuerySource.Query() => Todo;
        IQueryable<TagEntity> ITagQuerySource.Query() => Tags;
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
