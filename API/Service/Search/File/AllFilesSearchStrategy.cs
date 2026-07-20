using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;

namespace MyLife.Service.Search.File
{
    [Obsolete("业务复用,个别情况需要")]
    public class AllFilesSearchStrategy : ISearchStrategy<FileMetaEntity>
    {
        // 只有当 Uid 和 Title 全为空时，才应用此策略
        public bool IsMatch(Guid? guid = null, string? title = null)
            => guid == null && string.IsNullOrWhiteSpace(title);

        public async Task<IEnumerable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            Guid? guid = null, string? title = null)
        {
            // 对应你的：await fileService.TryReadListAsync()
            // 直接从已经被权限过滤过的 queryable 中拉取列表
            return await queryable.ToListAsync();
        }
    }
}
