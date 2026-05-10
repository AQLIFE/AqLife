using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Service.Interfaces;

namespace MyLife.Service.Strategies
{

    // 策略 A：按唯一 ID 精确搜索
    public class SearchByIdStrategy : IFileSearchStrategy
    {
        public bool IsMatch(string? title, Guid? id) => id.HasValue;
        public async Task<FileMetaEntity?> ExecuteAsync(IQueryable<FileMetaEntity> query, string? title, Guid? id)
            => await query.FirstOrDefaultAsync(f => f.Uuid == id);
    }

    // 策略 B：按文件名模糊搜索
    public class SearchByTitleStrategy : IFileSearchStrategy
    {
        public bool IsMatch(string? title, Guid? id) => !string.IsNullOrEmpty(title);
        public async Task<FileMetaEntity?> ExecuteAsync(IQueryable<FileMetaEntity> query, string? title, Guid? id)
            => await query.Where(f => f.FileName.Contains(title!)).FirstOrDefaultAsync();
    }
}
