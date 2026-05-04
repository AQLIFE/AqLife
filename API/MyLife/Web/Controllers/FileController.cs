using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLife.Data.Entity;
using MyLife.Data.Repository;
using MyLife.Service.Strategies;
using MyLife.Web.Provision;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("file")]
    public class FileController(FileSearchProvider fileProvider, FileService service, AppStorage storage, ILogger<FileController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<List<FileIndexEntity>> GetFileList()
            => await storage.File.ToListAsync();

        [HttpGet("search")]
        public async Task<IActionResult> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            var file = await fileProvider.FindFileAsync(title, id);
            return file == null ? NotFound() : Ok(file);
        }

        [HttpGet("download")]
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

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveFile(IFormFile file)
        {
            var innerFile = await service.HandleUploadAsync(file);

            return Ok(new { innerFile.FileHash, innerFile.FileName, file.Length });
        }
    }
}
