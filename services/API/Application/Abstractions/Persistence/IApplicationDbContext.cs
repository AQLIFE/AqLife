using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Domain.Entities.Favorite;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<FileMetaEntity> File { get; }
        DbSet<CorpusEntity> Corpus { get; }
        DbSet<AccountEntity> Accounts { get; }
        DbSet<SubscriptionEntity> Subscription { get; }
        DbSet<TodoEntity> Todo { get; }
        DbSet<DevelopmentPlanEntity> DevelopmentPlans { get; }
        DbSet<TodoDependencyEntity> TodoDependencies { get; }
        DbSet<RoadmapNodeLayoutEntity> RoadmapNodeLayouts { get; }
        DbSet<TagEntity> Tags { get; }
        DbSet<FileTagEntity> BlogTags { get; }
        DbSet<SystemStateEntity> SystemStates { get; }
        DbSet<FavoriteItemEntity> FavoriteItems { get; }
        DbSet<FavoriteMediaEntity> FavoriteMedia { get; }
        DbSet<FavoriteTagEntity> FavoriteTags { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<ITransaction> BeginTransactionAsync(CancellationToken ct);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
