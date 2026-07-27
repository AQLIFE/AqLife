using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.Persistence;
using MyLife.Application.Abstractions.Search;
using MyLife.Domain.CommandInterface;
using MyLife.Domain.Contracts;

namespace MyLife.Application.Search
{
    /// <summary>
    /// BaseSearch 是一个抽象类，提供了通用的搜索逻辑，允许子类通过映射查询条件和构建基础查询来实现特定的搜索功能。它使用策略模式来选择适当的搜索策略，并执行最终的查询操作。
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TEntityDto"></typeparam>
    /// <param name="storage"></param>
    /// <param name="searchStrategies"></param>
    public abstract class BaseSearch<TQuery, TEntity, TEntityDto, TSearchCriteria>(
    IApplicationDbContext storage,
    IEnumerable<ISearchStrategy<TEntity, TSearchCriteria>> searchStrategies
) : ISearch<TQuery, TEntity, TEntityDto>
    where TQuery : IQuery<IEnumerable<TEntityDto>>
    where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        protected abstract TSearchCriteria MapToCriteria(TQuery query);
        protected abstract Task<IQueryable<TEntity>> BuildBaseQueryAsync(IQueryable<TEntity> queryable, TQuery query);

        // 核心逻辑复用：不需要子类 override [cite: 204, 210]
        public async Task<IEnumerable<TEntity>> SearchAsync(TQuery query, CancellationToken ct)
        {
            var criteria = MapToCriteria(query);
            var strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(criteria));

            if (strategy == null) return [];

            // d. 获取通用 DbSet 并应用子类的 Include 逻辑 [cite: 164, 210]
            // 使用 .AsNoTracking() 确保检索操作的性能极致 [cite: 210]
            var baseQueryable = await BuildBaseQueryAsync(storage.Set<TEntity>().AsNoTracking(), query);

            // e. 执行最终的策略编译查询
            return await strategy.ExecuteAsync(baseQueryable, criteria, ct);
        }
    }
}
