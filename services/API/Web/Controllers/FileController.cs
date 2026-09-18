using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Web.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{

    [ApiController, Route("[Controller]"), Authorize]
    public class FileController(IMediator mediator) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<IEnumerable<FileDto>> SearchFile([FromQuery] FileQuery query, CancellationToken ct)
        => await mediator.Send(query, ct);// 已实现

        [HttpGet("preview"), AllowAnonymous]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> PreviewFile([FromQuery] PreviewFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);// 已实现
            return File(result.FileStream, result.ContentType);
        }

        [HttpGet("download")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
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

        [HttpPatch("CancelSchedule")]
        public async Task<Guid> CancelSchedule(CancelScheduledFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);
        /// <summary>
        /// 上传文件并设置延时发布
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("Publish"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> PublishScheduledFile([FromForm] PublishFileCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现

        [HttpPatch("schedule")]
        public async Task<Guid> ScheduleFile(ScheduledFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

    }
}
