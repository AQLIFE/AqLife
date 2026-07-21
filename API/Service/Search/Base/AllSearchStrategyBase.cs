using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Contracts;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;

namespace MyLife.Service.Search.Base
{
    public class AllSearchStrategyBase<TEntity, TSearchCriteria> : ISearchStrategy<TEntity, TSearchCriteria>
        where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        // 只有当 Uid 和 Title 全为空时，才应用此策略
        public virtual bool IsMatch(TSearchCriteria c)
            => c.UID is null && string.IsNullOrWhiteSpace(c.Keyword);

        public async Task<IEnumerable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            TSearchCriteria c,
            CancellationToken ct = default)
        =>await queryable.ToListAsync(ct);
        
    }
}
