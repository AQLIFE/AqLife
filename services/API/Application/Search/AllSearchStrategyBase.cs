using AqLife.Application.Abstractions.Search;
using AqLife.Shared.Contracts;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Search
{
    public class AllSearchStrategyBase<TEntity, TSearchCriteria>(IHttpContextAccessor httpContext) : ISearchStrategy<TEntity, TSearchCriteria>
        where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        protected private bool IsValid => httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        // 只有当 Uid 和 Title 全为空时，才应用此策略
        public virtual bool IsMatch(TSearchCriteria c)
            => c.UID is null && string.IsNullOrWhiteSpace(c.Keyword);

        public virtual IQueryable<TEntity> Execute(
            IQueryable<TEntity> queryable,
            TSearchCriteria c,
            CancellationToken ct = default)
        => queryable;
    }
}
