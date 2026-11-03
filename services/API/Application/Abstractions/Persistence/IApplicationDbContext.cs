using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Domain.Entities.Favorite;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        public DbSet<FileMetaEntity> File { get; }
        public DbSet<CorpusEntity> Corpus { get; }
        public DbSet<AccountEntity> Accounts { get; }
        public DbSet<SubscriptionEntity> Subscription { get; }
        public DbSet<TodoEntity> Todo { get; }
        public DbSet<TagEntity> Tags { get; }
        public DbSet<FileTagEntity> BlogTags { get; }
        public DbSet<SystemStateEntity> SystemStates { get; }
        public DbSet<FavoriteItemEntity> FavoriteItems { get; }
        public DbSet<FavoriteMediaEntity> FavoriteMedia { get; }
        public DbSet<FavoriteTagEntity> FavoriteTags { get; }



        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
        Task<ITransaction> BeginTransactionAsync(
        CancellationToken ct);

        DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
    }
}
