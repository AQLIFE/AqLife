using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using MyLife.Web.BusinessSecurity.RuntimeCheck;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("[Controller]"),Authorize]
    public class FileController(
        FileMapper mapper,
        FileService fileService) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<FileDto>?> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        => (title is null && id is null ? await fileService.TryReadListAsync() : await fileService.TryReadAsync(id, title))
            is IEnumerable<FileMetaEntity> target && target.Any() ? target.Select(e => mapper.ToDto(e!)) : null;

        [HttpGet("download"), AllowAnonymous]
        public async Task<IActionResult?> DownloadFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        {
            var (stream, contentType, fileName) = await fileService.GetFileInternalAsync(title, id);
            return User.Identity is not null && User.Identity.IsAuthenticated ? File(stream, contentType, fileName) : File(stream, contentType);
        }

        [HttpPost("receive"), Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<FileDto>> ReceiveFile(params IFormFile[] file)
        {
            var innerFile = await fileService.TryCreateAsync(file);
            
            return innerFile.Select(e=>mapper.ToDto(e));
        }
    }
}
