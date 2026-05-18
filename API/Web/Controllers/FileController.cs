using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Data.Repository;
using MyLife.Service.Implementations;
using MyLife.Service.Interfaces;
using MyLife.Service.Mappings;
using MyLife.Service.Strategies;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using MyLife.Web.Filters;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("[Controller]"), AllowAnonymous]
    public class FileController(
        IFileSearch fileSearch,
        IGenericsMapper<FileMetaEntity, FileDto> mapper,
        FileService fileService) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<FileDto>?> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        => (title is null && id is null? await fileSearch.FindAllAsync() : await fileSearch.FindFileAsync(title, id))
            is IEnumerable<FileMetaEntity> target ? target.Select(e => mapper.Desensitization(e)) : null;

        [HttpGet("download")]
        public async Task<IActionResult?> DownloadFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            var (stream, contentType, fileName) = await fileService.GetFileInternalAsync(title, id);
            return User.Identity is not null && User.Identity.IsAuthenticated ? File(stream, contentType, fileName) : File(stream, contentType);
        }

        [HttpPost("receive"), Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<FileDto> ReceiveFile(IFormFile file)
        {
            var innerFile = await fileService.HandleUploadAsync(file);
            return mapper.Desensitization(innerFile);
        }
    }
}
