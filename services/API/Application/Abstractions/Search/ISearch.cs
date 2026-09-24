using AqLife.Domain.Contracts;
using AqLife.Shared.IView;

namespace AqLife.Application.Abstractions.Search
{
    public interface ISearch<TQuery, TEntity, TEntityDto>
    //where TQuery : IQuery<IEnumerable<TEntityDto>>
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
