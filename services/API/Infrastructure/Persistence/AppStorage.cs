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
    public class AppStorage(DbContextOptions<AppStorage> options) : DbContext(options), IUnitOfWork, IModuleQuerySources, IAccountPersistence, ICategoryPersistence, ICorpusPersistence, IFavoritePersistence, IFilePersistence, ITagPersistence, IPlanPersistence
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
        IQueryable<CorpusEntity> ICorpusPersistence.Corpus => Corpus;
        async Task<CorpusEntity?> ICorpusPersistence.FindAsync(Guid uid, CancellationToken cancellationToken)
            => await Corpus.FindAsync([uid], cancellationToken);
        async Task ICorpusPersistence.AddCorpusAsync(CorpusEntity corpus, CancellationToken cancellationToken)
            => await Corpus.AddAsync(corpus, cancellationToken);
        void ICorpusPersistence.RemoveCorpus(CorpusEntity corpus) => Corpus.Remove(corpus);
        IQueryable<FavoriteItemEntity> IFavoriteItemQuerySource.Query() => FavoriteItems;
        IQueryable<FavoriteItemEntity> IFavoritePersistence.FavoriteItems => FavoriteItems;
        async Task IFavoritePersistence.AddFavoriteAsync(FavoriteItemEntity favorite, CancellationToken cancellationToken)
            => await FavoriteItems.AddAsync(favorite, cancellationToken);
        void IFavoritePersistence.RemoveFavorite(FavoriteItemEntity favorite) => FavoriteItems.Remove(favorite);
        IQueryable<FileMetaEntity> IFileMetaQuerySource.Query() => File;
        IQueryable<FileMetaEntity> IFilePersistence.Files => File;
        async Task<FileMetaEntity?> IFilePersistence.FindAsync(Guid uid, CancellationToken cancellationToken)
            => await File.FindAsync([uid], cancellationToken);
        async Task IFilePersistence.AddFileAsync(FileMetaEntity file, CancellationToken cancellationToken)
            => await File.AddAsync(file, cancellationToken);
        void IFilePersistence.RemoveFile(FileMetaEntity file) => File.Remove(file);
        async Task<Dictionary<string, Guid>> IFilePersistence.FindExistingFilesAsync(
            IEnumerable<string> hashes, bool isTemplate, CancellationToken cancellationToken)
            => await File.Where(file => hashes.Contains(file.FileHash) && file.IsTemplate == isTemplate)
                .ToDictionaryAsync(file => file.FileHash, file => file.UID, cancellationToken);
        IQueryable<DevelopmentPlanEntity> IDevelopmentPlanQuerySource.Query() => DevelopmentPlans;
        IQueryable<DevelopmentPlanEntity> IPlanPersistence.DevelopmentPlans => DevelopmentPlans;
        IQueryable<TodoEntity> IPlanPersistence.Todos => Todo;
        IQueryable<TodoDependencyEntity> IPlanPersistence.TodoDependencies => TodoDependencies;
        IQueryable<RoadmapNodeLayoutEntity> IPlanPersistence.RoadmapNodeLayouts => RoadmapNodeLayouts;
        async Task IPlanPersistence.AddDevelopmentPlanAsync(DevelopmentPlanEntity plan, CancellationToken cancellationToken)
            => await DevelopmentPlans.AddAsync(plan, cancellationToken);
        async Task IPlanPersistence.AddTodoAsync(TodoEntity todo, CancellationToken cancellationToken)
            => await Todo.AddAsync(todo, cancellationToken);
        async Task IPlanPersistence.AddTodoDependencyAsync(TodoDependencyEntity dependency, CancellationToken cancellationToken)
            => await TodoDependencies.AddAsync(dependency, cancellationToken);
        async Task IPlanPersistence.AddRoadmapNodeLayoutAsync(RoadmapNodeLayoutEntity layout, CancellationToken cancellationToken)
            => await RoadmapNodeLayouts.AddAsync(layout, cancellationToken);
        void IPlanPersistence.RemoveTodo(TodoEntity todo) => Todo.Remove(todo);
        void IPlanPersistence.RemoveTodoDependency(TodoDependencyEntity dependency) => TodoDependencies.Remove(dependency);
        IQueryable<TodoEntity> ITodoQuerySource.Query() => Todo;
        IQueryable<TagEntity> ITagQuerySource.Query() => Tags;
        IQueryable<TagEntity> ITagPersistence.Tags => Tags;
        async Task<TagEntity?> ITagPersistence.FindAsync(Guid uid, CancellationToken cancellationToken)
            => await Tags.FindAsync([uid], cancellationToken);
        async Task ITagPersistence.AddTagAsync(TagEntity tag, CancellationToken cancellationToken)
            => await Tags.AddAsync(tag, cancellationToken);
        void ITagPersistence.RemoveTag(TagEntity tag) => Tags.Remove(tag);
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
