using MyLife.Domain.Contracts;
using MyLife.Domain.Entities;

namespace MyLife.Service.Interfaces
{
    public interface ISearchStrategy<TEntity> where TEntity : IEntity
    {
        // 判断当前 Query 是否匹配该策略
        bool IsMatch(Guid? UID = null, string? Title = null);

        // 执行数据库查询
        Task<IEnumerable<TEntity>> ExecuteAsync(
            IQueryable<TEntity> queryable,
            Guid? UID = null, string? Title = null);
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
