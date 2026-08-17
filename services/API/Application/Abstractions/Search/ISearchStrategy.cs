using MyLife.Domain.Contracts;

namespace MyLife.Application.Abstractions.Search
{

    public interface ISearchStrategy<TEntity, TSearchCriteria> where TEntity : IEntity where TSearchCriteria : ISearchCriteria
    {
        // 判断当前 Query 是否匹配该策略
        bool IsMatch(TSearchCriteria criteria);
        Task<IEnumerable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            TSearchCriteria criteria,
            CancellationToken ct = default);
    }

}
