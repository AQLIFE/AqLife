using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Shared.Contracts;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Search
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
    IHttpContextAccessor httpContext,
    IEnumerable<ISearchStrategy<TEntity, TSearchCriteria>> searchStrategies
) : ISearch<TQuery, TEntity, TEntityDto>
    //where TQuery : IQuery<TEntityDto> // 部分业务 不需要返回DTO
    where TEntity : class, IEntity
    where TSearchCriteria : ISearchCriteria
    {
        /// <summary>
        /// 身份验证状态 : 默认实现
        /// </summary>
        protected private bool IsValid => httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;


        /// <summary>
        /// 获取查询条件：子类实现
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected private abstract TSearchCriteria MapToCriteria(TQuery query);
        /// <summary>
        /// 构建查询：子类实现
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        protected private abstract IQueryable<TEntity> BuildBaseQuery(IQueryable<TEntity> queryable, TQuery query);
        /// <summary>
        /// 设置排序：子类实现
        /// </summary>
        /// <param name="queryble"></param>
        /// <returns></returns>
        protected private abstract IQueryable<TEntity> ApplyDefaultOrder(IQueryable<TEntity> queryble, TQuery query);

        /// <summary>
        /// 核心逻辑复用：不需要子类 override 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="RequestCheckException"></exception>
        public async Task<IQueryable<TEntity>> SearchAsync(TQuery query, CancellationToken ct)
        {
            TSearchCriteria criteria = MapToCriteria(query);
            ISearchStrategy<TEntity, TSearchCriteria> strategy = searchStrategies.FirstOrDefault(s => s.IsMatch(criteria)) ?? throw new RequestCheckException("无法完成搜索，请反馈");

            IQueryable<TEntity> queryable = BuildBaseQuery(storage.Set<TEntity>().AsNoTracking(), query);
            queryable = ApplyDefaultOrder(queryable, query);
            queryable = strategy.Execute(queryable, criteria, ct);

            return queryable;
        }
        /// <summary>
        /// 仅限子类可重写和使用
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="pageQuery"></param>
        /// <returns></returns>
        protected private virtual IQueryable<TEntity> BuildPageQuery(IQueryable<TEntity> queryable, IPageQuery pageQuery)
            => queryable
                .Skip((pageQuery.Page - 1) * pageQuery.PageSize)
                .Take(pageQuery.PageSize + 1);
        
        /// <summary>
        /// 执行分页查询 : 加载实体
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<PageResult<TEntity>> SearchPageAsync(TQuery query, CancellationToken ct)
        {
            IQueryable<TEntity> queryable = await this.SearchAsync(query, ct);
            // ③ 分页
            if (query is IPageQuery pageQuery)
            {
                IQueryable<TEntity> pageQueryable = BuildPageQuery(queryable, pageQuery);
                //IEnumerable<TEntity> pageResult = queryable.Take(pageQuery.PageSize);
                return new PageResult<TEntity>(pageQueryable,pageQuery);
            }
            else throw new("未配置业务属性，无法分页");
        }
                
    }
}
