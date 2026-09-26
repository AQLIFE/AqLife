using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Search
{
    /// <summary>
    /// 提供一个抽象的搜索策略基类，用于根据指定的搜索条件对实体进行过滤和查询。任意一个检索条件不为空即可触发
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class FilteredSearchStrategyBase<TEntity, TSearchCriteria> : ISearchStrategy<TEntity, TSearchCriteria>
    where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        public virtual  bool IsMatch(TSearchCriteria c)
            => c.UID is not null || !string.IsNullOrWhiteSpace(c.Keyword);
        public virtual async Task<IQueryable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            TSearchCriteria c,
            CancellationToken ct = default)
        {
            if (c.UID is Guid id)
                return queryable.Where(e => e.UID == id);
            else if (!string.IsNullOrWhiteSpace(c.Keyword))
                return ApplyKeywordFilter(queryable, c.Keyword.Trim());
            else throw new RequestFailException("不合规的操作，该请求不应被处理");
        }
        protected abstract IQueryable<TEntity> ApplyKeywordFilter(
            IQueryable<TEntity> queryable, string keyword);
    }
}
