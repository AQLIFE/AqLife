using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Service.Command;
using MyLife.Shared.DTOs;
using MyLife.Web.BusinessSecurity.RuntimeCheck;

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

        [HttpGet("download"), Authorize]
        public async Task<IActionResult> DownloadFile([FromQuery] DownloadFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);// 已实现
            return File(result.FileStream, result.ContentType, result.FileName);
        }

        [HttpPost("Upload"), Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> UploadFile(CreateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现


        [HttpPut, Authorize, ServiceFilter(typeof(FileUploadFilter))]
        public async Task<Guid> UpdateFile(UpdateFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现


        [HttpDelete]
        public async Task DeleteFile(DeleteFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);// 已实现
    }
}
