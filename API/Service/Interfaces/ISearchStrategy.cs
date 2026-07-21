using MyLife.Domain.Contracts;
using MyLife.Domain.Entities;

namespace MyLife.Service.Interfaces
{
    public interface ISearchCriteria
    {
        Guid? UID { get; init; }
        string? Keyword { get; init; }
    }
    public readonly record struct EntitySearchCriteria(Guid? UID, string? Keyword): ISearchCriteria;

    public interface ISearchStrategy<TEntity, TSearchCriteria> where TEntity : IEntity where TSearchCriteria:ISearchCriteria
    {
        // 判断当前 Query 是否匹配该策略
        bool IsMatch(TSearchCriteria criteria);
        Task<IEnumerable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            TSearchCriteria criteria,
            CancellationToken ct = default);
    }

    public interface IAccountSearchStrategy
    {
        // 判断当前 Query 是否匹配该策略
        bool IsMatch(Guid? UID = null);

        // 执行数据库查询
        Task<IEnumerable<AccountEntity>> ExecuteAsync(
            IQueryable<AccountEntity> queryable,
            Guid? UID = null);
    }

}
