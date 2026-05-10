using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Interfaces;
using MyLife.Service.Strategies;
using MyLife.Shared.Accident;
using MyLife.Shared.Config;

namespace MyLife.Service.Implementations
{
    public class FileService(
        IEnumerable<IUploadCheckStrategyAsync> _strategiesAsync, // 注入我们之前写的校验策略
        IOptions<FileOption> _policy,                  // 注入配置
        AppStorage _storage,
        UploadContext _upContext,
        ILogger<FileService> _logger,
        IFileSearch _fileProvider)
    {
        /// <summary>
        /// 生成文件流,提供给 Controller 层直接返回给客户端, 这样可以解耦 IO 逻辑和 Web 层的细节
        /// </summary>
        /// <param name="title"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<(Stream stream, string contentType, string fileName)> GetFileDownloadStreamAsync(string? title, Guid? id)
        {
            var fileInfo = await _fileProvider.FindFileAsync(title, id)
                           ?? throw new KeyNotFoundException("数据库无记录");

            var fullPath = Path.Combine(_policy.Value.StoragePath, fileInfo.DesensitizationName);

            if (!System.IO.File.Exists(fullPath))
                throw new FileNotFoundException("磁盘物理文件丢失", fullPath);

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return (stream, "application/octet-stream", fileInfo.FileName);
        }

        public async Task<FileMetaEntity> HandleUploadAsync(IFormFile file)
        {
            foreach(var item in _strategiesAsync)
            {
                (bool IsValid, string Message) = await item.CheckAsync(file, _policy.Value);
                if (!IsValid) throw new OperateBusinessLogicCheckException(Message);
            };
            var fileMeta = new FileMetaEntity(file, _upContext.FileHash ?? throw new OperateTransactionFailedException("读取hash失败"));

            var targetPath = Path.Combine(_policy.Value.StoragePath, fileMeta.DesensitizationName);
            await SaveFile(file, targetPath);


            _storage.File.Add(fileMeta);
            await _storage.SaveChangesAsync();
            _logger.LogInformation("文件上传成功: {Id}", file.FileName);
            return fileMeta;
        }

        /// <summary>
        /// 保存文件到本地,并在保存过程中计算文件的哈希值, 避免重复读取文件两次 (一次计算哈希, 一次保存), 提高性能
        /// </summary>
        /// <param name="file">源文件</param>
        /// <param name="targetPath">目标路径</param>
        /// <returns>文件的哈希值</returns>
        private static async Task SaveFile(IFormFile file, string targetPath)
        {
            using var fs = new FileStream(targetPath, FileMode.Create);
            await file.CopyToAsync(fs);
        }
    }
}