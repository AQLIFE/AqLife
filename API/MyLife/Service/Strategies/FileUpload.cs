using MyLife.Service.Interfaces;
using MyLife.Shared.Config;

namespace MyLife.Service.Strategies
{
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
                : (false, $"文件大小超出限制 (最大允许: {policy.MaxFileSize} {(limit >= 1024 ? "KB" : "B")}),实际{file.Length} B");
        }
    }
}
