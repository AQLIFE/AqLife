using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Core.Func;
using MyLife.Core.Provider;
using MyLife.Entity;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common;
using System.Net;

namespace MyLife.Controllers
{

    [ApiController, Route("file")]
    public class FileController(AppStorage storage, ILogger<FileController> logger, IOptions<FilePolicy> options, IWebHostEnvironment webHost) : ControllerBase
    {
        [HttpGet]
        public async Task<List<FileIndexEntity>> GetFileList()
            => await storage.File.ToListAsync();

        [HttpGet("download")]
        public async Task<IActionResult?> SearchFile([FromServices] IEnumerable<IFileSearchStrategy> strategies,[FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            if (!options.Value.AllowDownload) return Forbid("下载功能已关闭");

            if (string.IsNullOrEmpty(title) && id is null) return BadRequest("参数无效");

            // 2. 找到匹配的策略并执行
            var strategy = strategies.FirstOrDefault(s => s.IsMatch(title, id));
            if (strategy == null) return NotFound("未找到合适的搜索方式");

            var result = await strategy.ExecuteAsync(storage.File.AsQueryable(), title, id);

            // 3. 处理文件流返回
            if (result == null) return NotFound("未找到匹配的文件");

            var fullPath = Path.Combine(webHost.ContentRootPath, options.Value.StoragePath, result.SavePath);

            if (!System.IO.File.Exists(fullPath))
            {
                logger.LogError(@"[Serilog][{@LogType}]=>数据库记录存在但物理文件缺失: {Path}", LogType.ApiType, fullPath);
                return NotFound("物理文件丢失");
            }

            var contentType = "application/octet-stream"; // 也可以根据后缀名获取准确的 MIME
            return PhysicalFile(fullPath, contentType, result.FileName);
        }

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveFile(IFormFile file, [FromServices] IEnumerable<IUploadCheckStrategy> checkers)
        {
            foreach (var checker in checkers)
            {
                var (isValid, message) = checker.Check(file, options.Value);
                if (!isValid)
                {
                    logger.LogWarning(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, message);
                    return BadRequest(message);
                }
            }

            // 1. 生成唯一文件名 (防止同名覆盖)
            //var trustedFileNameForDisplay = WebUtility.HtmlEncode(file.FileName);
            var untrustedFileName = Path.GetFileName(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(untrustedFileName)}";
            var filePath = Path.Combine(options.Value.StoragePath, storedFileName);


            FilePolicyProvider.EnsureStorageDirectoryCreated(webHost.ContentRootPath, options.Value.StoragePath);
            // 3. 计算哈希值 (示例使用 SHA256)

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

            storage.File.Add(fileMeta);
            await storage.SaveChangesAsync();

            return Ok(new { fileMeta.FileHash, fileMeta.FileName,file.Length });
        }
    }
}
