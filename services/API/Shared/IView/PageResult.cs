using AqLife.Shared.Contracts.CommandInterface;

namespace AqLife.Shared.IView
{
    public record PageResult<TData>(
        List<TData> Items,
        int Page,
        int PageSize,
        bool HasMore = true)
    {
        /// <summary>
        /// 基于 SQL 分页查询 构建 PageResult ; 副作用: 会多执行一次SQL查询
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        public PageResult(IQueryable<TData> queryable, IPageQuery query) : this(
                [.. queryable.Take(query.PageSize)],
                query.Page,
                query.PageSize, queryable.Count() > query.PageSize)
        { }

        /// <summary>
        /// 基于加载后的数据构建PageResult ; 副作用: 加载到内存的数据
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        public PageResult(List<TData> Items, IPageQuery query) : this(
                Items,
                query.Page,
                query.PageSize, Items.Count > query.PageSize)
        { }
    }
}
