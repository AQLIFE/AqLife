using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Core.Provider;
using MyLife.Entity;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Common;
using System.Net;

namespace MyLife.Controllers
{

    [ApiController,Route("file")]
    public class FileController(AppStorage storage,ILogger<FileController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<List<FileIndexEntity>> GetFileList()
            => await storage.File.ToListAsync();

        //[HttpGet("search"),Obsolete("待定")]
        //public async Task<FileIndexEntity?> SearchFile([FromQuery]string? title, [FromQuery]Guid? id)
        //{
        //    if ( string.IsNullOrEmpty(title) && id is null)
        //    {
        //        logger.LogWarning(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, "检查到无效请求");

        //    }
        //    else if (storage.File.Count() > 0 )
        //    {
        //        var query = storage.File.AsQueryable();
        //        if (!string.IsNullOrEmpty(title))
        //        {
        //            query = query.Where(f => f.FileName.Contains(title));
        //        }
        //        if (id is not null)
        //        {
        //            query = query.Where(f => f.Uuid == id);
        //        }
        //        var result = await query.FirstOrDefaultAsync();
        //        if (result is null)
        //        {
        //            logger.LogInformation(@"[Serilog][{@LogType}]=>{@LogDesc}", LogType.ApiType, "未找到匹配的文件");
        //            //return new ProblemDetails { Title = "未找到", Detail = "没有找到匹配的文件" };
        //        }
        //        return result;
        //    }
        //    //return new ProblemDetails { Title = "无数据", Detail = "当前没有任何文件记录" }.ToString();
        //}

        //[HttpPost("receive")]
        //public async Task<bool> ReceiveFile(IFormFile file)
        //{
        //    if (file == null || file.Length == 0) return BadRequest("请选择文件");

        //    // 1. 生成唯一文件名 (防止同名覆盖)
        //    var trustedFileNameForDisplay = WebUtility.HtmlEncode(file.FileName);
        //    var untrustedFileName = Path.GetFileName(file.FileName);
        //    var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(untrustedFileName)}";
        //    var filePath = Path.Combine(_uploadDir, storedFileName);

        //    // 2. 保存物理文件
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // 3. 计算哈希值 (示例使用 SHA256)
        //    string hash;
        //    using (var sha256 = System.Security.Cryptography.SHA256.Create())
        //    {
        //        using var stream = System.IO.File.OpenRead(filePath);
        //        var hashBytes = sha256.ComputeHash(stream);
        //        hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        //    }

        //    // 4. 存储元数据到数据库
        //    var fileMeta = new FileIndexEntity
        //    {
        //        FileName = untrustedFileName,
        //        StoredFileName = storedFileName,
        //        FilePath = filePath,
        //        Hash = hash,
        //        Size = file.Length,
        //        UploadTime = DateTime.UtcNow
        //    };

        //    storage.File.Add(fileMeta);
        //    await storage.SaveChangesAsync();

        //    return Ok(new { fileMeta.Id, fileMeta.FileName });
        //}
    }
}
