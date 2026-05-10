using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Implementations;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("[Controller]"),AllowAnonymous]
    public class FileController(IGenericsMapper<FileMetaEntity, FileDto> mapper, IFileSearch fileProvider, FileService service, AppStorage storage, ILogger<FileController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<List<FileDto>> GetFileList()
        => await storage.File.AsNoTracking().Select(e => mapper.Desensitization(e)).ToListAsync();

        [HttpGet("search")]
        public async Task<FileMetaEntity?> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        => await fileProvider.FindFileAsync(title, id);


        [HttpGet("download"),Authorize]
        public async Task<IActionResult?> DownloadFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            try
            {
                var (stream, contentType, fileName) = await service.GetFileDownloadStreamAsync(title, id);
                // 使用 FileStreamResult 自动处理流的关闭
                return File(stream, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                logger.LogCritical(ex, "物理文件缺失");
                return NotFound("文件已丢失");
            }
        }

        [HttpPost("receive"),Authorize]
        public async Task<IActionResult> ReceiveFile(IFormFile file)
        {
            var innerFile = await service.HandleUploadAsync(file);

            return Ok(new { innerFile.FileHash, innerFile.FileName, file.Length });
        }
    }
}
