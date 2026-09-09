using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Search
{
    public class AllSearchStrategyBase<TEntity, TSearchCriteria> : ISearchStrategy<TEntity, TSearchCriteria>
        where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        // 只有当 Uid 和 Title 全为空时，才应用此策略
        public virtual bool IsMatch(TSearchCriteria c)
            => c.UID is null && string.IsNullOrWhiteSpace(c.Keyword);

        public virtual async Task<IEnumerable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            TSearchCriteria c,
            CancellationToken ct = default)
        => await queryable.ToListAsync(ct);

    }
}
