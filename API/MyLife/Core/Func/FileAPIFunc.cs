using Microsoft.EntityFrameworkCore;
using MyLife.Core.Provider;
using MyLife.Entity;

namespace MyLife.Core.Func
{
    public interface IFileSearchStrategy
    {
        // 判断当前参数是否适用于该策略
        bool IsMatch(string? title, Guid? id);
        // 执行查询逻辑
        Task<FileIndexEntity?> ExecuteAsync(IQueryable<FileIndexEntity> query, string? title, Guid? id);
    }

    // 策略 A：按唯一 ID 精确搜索
    public class SearchByIdStrategy : IFileSearchStrategy
    {
        public bool IsMatch(string? title, Guid? id) => id.HasValue;
        public async Task<FileIndexEntity?> ExecuteAsync(IQueryable<FileIndexEntity> query, string? title, Guid? id)
            => await query.FirstOrDefaultAsync(f => f.Uuid == id);
    }

    // 策略 B：按文件名模糊搜索
    public class SearchByTitleStrategy : IFileSearchStrategy
    {
        public bool IsMatch(string? title, Guid? id) => !string.IsNullOrEmpty(title);
        public async Task<FileIndexEntity?> ExecuteAsync(IQueryable<FileIndexEntity> query, string? title, Guid? id)
            => await query.Where(f => f.FileName.Contains(title!)).FirstOrDefaultAsync();
    }

    public interface IUploadCheckStrategy
    {
        // 执行校验逻辑
        (bool IsValid, string Message) Check(IFormFile file, FilePolicy policy);
    }

    // 策略 1：权限检查
    public class UploadPermissionCheck : IUploadCheckStrategy
    {
        public (bool IsValid, string Message) Check(IFormFile file, FilePolicy policy) =>
            policy.AllowUpload ? (true, string.Empty) : (false, "服务器已关闭上传功能");
    }

    // 策略 2：后缀名检查
    public class ExtensionCheck : IUploadCheckStrategy
    {
        public (bool IsValid, string Message) Check(IFormFile file, FilePolicy policy)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return policy.AllowedExtensions.Contains(ext)
                ? (true, string.Empty)
                : (false, $"不支持的文件类型: {ext}");
        }
    }

    // 策略 3：大小检查（使用你的指数幂逻辑）
    public class SizeCheck : IUploadCheckStrategy
    {
        public (bool IsValid, string Message) Check(IFormFile file, FilePolicy policy)
        {
            long limit = (long)policy.MaxFileSize << policy.StorageUnit;
            // 等于 2^ StorageUnit * MaxFileSize
            // dev 配置 给到100 KB 就是 100 * 2^10 字节

            return file.Length <= limit
                ? (true, string.Empty)
                : (false, $"文件大小超出限制 (最大允许: {policy.MaxFileSize} {(limit>=1024?"KB":"B")}),实际{file.Length} B");
        }
    }
}
