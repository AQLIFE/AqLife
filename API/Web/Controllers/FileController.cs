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

    [ApiController, Route("[Controller]"), AllowAnonymous]
    public class FileController(IGenericsMapper<FileMetaEntity, FileDto> mapper, IFileSearch fileProvider, FileService service, AppStorage storage, ILogger<FileController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<List<FileDto>> GetFileList()
        => await storage.File.AsNoTracking().Select(e => mapper.Desensitization(e)).ToListAsync();

        [HttpGet("search")]
        public async Task<FileMetaEntity?> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        => await fileProvider.FindFileAsync(title, id);

        [HttpGet("preview")]
        public async Task<IActionResult?> Preview([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            var (stream, contentType, _) = await service.GetFileInternalAsync(title, id);
            return File(stream, contentType);
        }


        [HttpGet("download"), Authorize]
        public async Task<IActionResult?> DownloadFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            var (stream, contentType, fileName) = await service.GetFileInternalAsync(title, id);
            return File(stream, contentType, fileName);
        }

        [HttpPost("receive"), Authorize]
        public async Task<FileDto> ReceiveFile(IFormFile file)
        {
            var innerFile = await service.HandleUploadAsync(file);

            return mapper.Desensitization(innerFile);
        }
    }
}
