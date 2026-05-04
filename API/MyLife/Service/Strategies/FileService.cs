using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MyLife.Data.Entity;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Shared.Config;
using MyLife.Web.Provision;

namespace MyLife.Service.Strategies
{
    public class FileService(
        IEnumerable<IUploadCheckStrategy> _strategies, // 注入我们之前写的校验策略
        IOptions<FilePolicy> _policy,                  // 注入配置
        AppStorage _storage,                           // 注入数据库
        ILogger<FileService> _logger,
        IFileSearchProvider _fileProvider)
    {
        // 返回一个包含流、文件名和 MIME 类型的 DTO
        public async Task<(Stream stream, string contentType, string fileName)> GetFileDownloadStreamAsync(string? title, Guid? id)
        {
            var fileInfo = await _fileProvider.FindFileAsync(title, id)
                           ?? throw new KeyNotFoundException("数据库无记录");

            var fullPath = Path.Combine(_policy.Value.StoragePath, fileInfo.SavePath);

            if (!System.IO.File.Exists(fullPath))
                throw new FileNotFoundException("磁盘物理文件丢失", fullPath);

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return (stream, "application/octet-stream", fileInfo.FileName);
        }

        public async Task<FileIndexEntity> HandleUploadAsync(IFormFile file)
        {
            // 1. 运行校验策略 (解构了复杂的校验逻辑)
            foreach (var strategy in _strategies)
            {
                var (isValid, msg) = strategy.Check(file, _policy.Value);
                if (!isValid) throw new Exception(msg); // 抛出自定义异常，由全局异常处理器捕获
            }

            // 2. 处理物理存储 (解构了 IO 逻辑)

            var untrustedFileName = Path.GetFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(untrustedFileName)}";
            var filePath = Path.Combine(_policy.Value.StoragePath, storedFileName);
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // 2. 创建加密流，将 fileStream 包装起来
                // CryptoStreamMode.Write 表示我们要向里面写数据，同时计算哈希
                using var cryptoStream = new System.Security.Cryptography.CryptoStream(
                    fileStream, sha256, System.Security.Cryptography.CryptoStreamMode.Write);

                // 3. 将上传的文件流直接拷贝到 cryptoStream
                // 这一步会同时触发：写入硬盘 + 哈希计算
                await file.CopyToAsync(cryptoStream);

                // 4. 必须手动刷新，确保所有数据都已处理完成并写入底层流
                await cryptoStream.FlushFinalBlockAsync();
            }
            var hash = BitConverter.ToString(sha256.Hash!).Replace("-", "").ToLowerInvariant();

            // 4. 存储元数据到数据库
            var fileMeta = new FileIndexEntity
            {
                FileName = untrustedFileName,
                FileSize = (ulong)file.Length,
                FileHash = hash,
                SavePath = storedFileName
            };

            _storage.File.Add(fileMeta);
            await _storage.SaveChangesAsync();
            _logger.LogInformation("文件上传成功: {Id}", file.FileName);
            return fileMeta;
        }
    }
}