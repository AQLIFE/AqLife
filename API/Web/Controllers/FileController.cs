using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyLife.Data.Entities;
using MyLife.Service.Command;
using MyLife.Service.EntityService;
using MyLife.Service.Mappings;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using MyLife.Shared.Options;
using MyLife.Web.BusinessSecurity.RuntimeCheck;
using System.Security.Cryptography;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("[Controller]"), Authorize]
    public class FileController(IMediator mediator) : ControllerBase
    {
        //[HttpGet, AllowAnonymous]
        //public async Task<IEnumerable<FileDto>?> SearchFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        //=> (title is null && id is null ? await fileService.TryReadListAsync() : await fileService.TryReadAsync(id, title))
        //    is IEnumerable<FileMetaEntity> target && target.Any() ? target.Select(e => fileMapper.ToDto(e!)) : null;

        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<FileMetadataDto>> SearchFile([FromQuery] GetFileMetadataQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);

        //[HttpGet("download"), AllowAnonymous]
        //public async Task<IActionResult?> DownloadFile([FromQuery] string? title = null, [FromQuery] Guid? id = null)
        //{
        //    var (stream, contentType, fileName) = await fileService.GetFileInternalAsync(title, id);
        //    return User.Identity is not null && User.Identity.IsAuthenticated ? File(stream, contentType, fileName) : File(stream, contentType);
        //}
        [HttpGet("preview"), AllowAnonymous]
        public async Task<IActionResult> PreviewFile([FromQuery] PreviewFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);
            return File(result.FileStream, result.ContentType);
        }

        //[HttpPost("receive"), Authorize, ServiceFilter(typeof(FileUploadFilter))]
        //public async Task<IEnumerable<Guid>> ReceiveFile(params IFormFile[] file)
        //{
        //    var innerFile = await fileService.TryCreateAsync(file);

        //    return innerFile.Select(e=>fileMapper.ToDto(e).UID);
        //}

        [HttpPost("receive"), Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> ReceiveFile(CreateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);


        [HttpPut, Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<Guid> UpdateFile(UpdateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);


        [HttpDelete]
        public async Task DeleteFile(DeleteFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);
    }
}
