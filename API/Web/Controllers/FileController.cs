using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Application.Command;
using MyLife.Shared.DTOs;
using MyLife.Web.Middlewares;

namespace MyLife.Web.Controllers
{

    [ApiController, Route("[Controller]"), Authorize]
    public class FileController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<FileMetadataDto>> SearchFile([FromQuery] GetFileMetadataQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);// 已实现

        [HttpGet("preview"), AllowAnonymous]
        public async Task<IActionResult> PreviewFile([FromQuery] PreviewFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);// 已实现
            return File(result.FileStream, result.ContentType);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] DownloadFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);// 已实现
            return File(result.FileStream, result.ContentType, result.FileName);
        }

        [HttpPost("Upload"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> UploadFile([FromForm] CreateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现


        [HttpPatch, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<Guid> UpdateFile([FromForm] UpdateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现

        [HttpPatch("tag")]
        public async Task<Guid> UpdateFileTag(UpdateFileTagCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteFile(DeleteFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);// 已实现
    }
}
