using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Shared.Options;

namespace MyLife.Service.Strategies
{
    // 策略 1：权限检查
    public class UploadPermissionCheck : IUploadCheckStrategyAsync
    {
        public async Task<(bool IsValid, string Message)> CheckAsync(IFormFile file, FileOption policy)
            => policy.AllowUpload ? (true, string.Empty) : (false, "服务器已关闭上传功能");
    }




    // 策略 2：后缀名检查
    public class ExtensionCheck : IUploadCheckStrategyAsync
    {
        public async Task<(bool IsValid, string Message)> CheckAsync(IFormFile file, FileOption policy)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return policy.AllowedExtensions.Contains(ext)
                ? (true, string.Empty)
                : (false, $"不支持的文件类型: {ext}");
        }
    }

    // 策略 3：大小检查（使用你的指数幂逻辑）
    public class SizeCheck : IUploadCheckStrategyAsync
    {
        public async Task<(bool IsValid, string Message)> CheckAsync(IFormFile file, FileOption policy)
        {
            long limit = (long)policy.MaxFileSize << policy.StorageUnit;
            // 等于 2^ StorageUnit * MaxFileSize
            // dev 配置 给到100 KB 就是 100 * 2^10 字节

            return file.Length <= limit
                ? (true, string.Empty)
                : (false, $"文件大小超出限制 (最大允许: {policy.MaxFileSize} {(limit >= 1024 ? "KB" : "B")}),实际{file.Length} B");
        }
    }

    /// <summary>
    /// 检查文件是否重复（通过计算文件的哈希值并与数据库中已有文件的哈希值进行比较）
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="context"></param>
    public class UploadFileEffectivenessCheck(AppStorage storage, UploadContext context) : IUploadCheckStrategyAsync
    {
        public async Task<(bool IsValid, string Message)> CheckAsync(IFormFile file, FileOption policy)
        {
            context.FileHash = await CalculateHashAsync(file);

            if (await storage.File.AsNoTracking().Where(e => e.FileName == file.FileName).FirstOrDefaultAsync() is FileMetaEntity meta)
                if (meta.FileHash == context.FileHash)
                    return (false, "文件重复");
            return (true, string.Empty);
            // 文件不存在和文件存在但哈希不同都算有效
        }
        private static async Task<string> CalculateHashAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
    public class UploadContext
    {
        public string? FileHash { get; set; }
    }
}
