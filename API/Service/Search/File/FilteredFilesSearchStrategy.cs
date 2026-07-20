using Microsoft.EntityFrameworkCore;
using MyLife.Domain.Entities;
using MyLife.Service.Interfaces;

namespace MyLife.Service.Search.File
{
    public class FilteredFilesSearchStrategy : ISearchStrategy<FileMetaEntity>
    {
        // 只要有一个有值，就应用此策略
        public bool IsMatch(Guid? UID = null, string? Title = null)
            => UID != null || !string.IsNullOrWhiteSpace(Title);

        public async Task<IEnumerable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            Guid? UID = null, string? Title = null)
        {
            // 对应你的：await fileService.TryReadAsync(id, title)
            if (UID != null)
            {
                queryable = queryable.Where(e => e.UID == UID);
            }

            else if (!string.IsNullOrWhiteSpace(Title))
            {
                queryable = queryable.Where(e => e.FileName.Contains(Title));
            }

            return await queryable.ToListAsync();
        }
    }
}
