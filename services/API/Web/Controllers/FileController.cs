using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLife.Domain.Command;
using MyLife.Shared.IView;
using MyLife.Web.Middlewares;

namespace MyLife.Web.Controllers
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

        /// <summary>
        /// 上传文件并设置延时发布
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("PublishScheduled"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> PublishScheduledFile([FromForm] PublishScheduledCommand command, CancellationToken ct)
        => await mediator.Send(command, ct);// 已实现

        /// <summary>
        /// 手动发布特定文件
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("Publish"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<Guid> PublishScheduledFile(PublishFileCommand command, CancellationToken ct)
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
