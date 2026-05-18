using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Shared.Options;

namespace MyLife.Service.Strategies
{
    /// <summary>
    /// 同步策略 1：权限检查
    /// </summary>
    /// <param name="options"></param>
    public class UploadPermissionCheck(IOptions<FilePolicyOption> options) : IUploadStrategy
    {
        public (bool IsValid, string Message) Check(IFormFile file)
            => options.Value.AllowedUpload.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()) ? (true, string.Empty) : (false, "不允许上传的文件类型");
    }
    /// <summary>
    /// 同步策略 3：大小检查（使用你的指数幂逻辑）
    /// </summary>
    /// <param name="options"></param>
    public class UploadSizeCheck(IOptions<FilePolicyOption> options) : IUploadStrategy
    {
        public (bool IsValid, string Message) Check(IFormFile file)
        {
            long limit = (long)options.Value.MaxFileSize << options.Value.StorageUnit;
            // 等于 2^ StorageUnit * MaxFileSize
            // dev 配置设置到100 KB=> 100 * 2^10 字节

            return file.Length <= limit
                ? (true, string.Empty)
                : (false, $"文件大小超出限制 (最大允许: {options.Value.MaxFileSize} {(limit >= 1024 ? "KB" : "B")}),实际{file.Length} B");
        }
    }

    /// <summary>
    /// 异步策略1 : 检查文件是否重复（通过计算文件的哈希值并与数据库中已有文件的哈希值进行比较）
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="context"></param>
    public class UploadFileEffectivenessCheck(AppStorage storage, UploadContext context) : IUploadStrategyAsync
    {
        public async Task<(bool IsValid, string Message)> CheckAsync(IFormFile file)
        {
            context.FileHash = await CalculateHashAsync(file);

            if (await storage.File.AsNoTracking().AnyAsync(e => e.FileHash == context.FileHash) )
                    return (false, "文件重复");
            return (true, string.Empty);
            // 文件不存在 和 文件存在但哈希不同 都算有效
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

    /// <summary>
    /// 下载策略1
    /// </summary>
    /// <param name="options"></param>
    public class DownloadPermissionCheck(IOptions<FilePolicyOption> options) : IDownloadStrategy
    {
        public (bool IsValid, string Message) Check(string ext)
            => options.Value.AllowedDownload.Contains(ext) ? (true, string.Empty) : (false, "不允许下载的文件类型");
    }

}
