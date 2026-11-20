using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.Contracts;

namespace AqLife.Application.Abstractions.Search
{
    public interface ISearch<TQuery,TReseponse, TEntity>
    where TQuery : IQuery<TReseponse> // 备用吧,等待未来实现
    where TEntity : IEntity
    {
        Task<IQueryable<TEntity>> SearchAsync(TQuery query, CancellationToken ct);
        /// <summary>
        /// 搜索分页
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PageResult<TEntity>> SearchPageAsync(TQuery query, CancellationToken ct);
    }
}
