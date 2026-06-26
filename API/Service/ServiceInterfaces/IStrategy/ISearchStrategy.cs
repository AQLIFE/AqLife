using MyLife.Data.Entities;

namespace MyLife.Service.ServiceInterfaces.IStrategy
{
    public interface ISearchStrategy
    {
        // 判断当前 Query 是否匹配该策略
        bool IsMatch(Guid? UID = null, string? Title = null);

        // 执行数据库查询
        Task<IEnumerable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            Guid? UID = null, string? Title = null);
    }

}
