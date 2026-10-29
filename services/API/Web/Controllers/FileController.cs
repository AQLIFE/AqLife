using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Web.Middlewares;
using AqLife.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqLife.Web.Controllers
{
    [ApiController, Route("[Controller]"), Authorize]
    public class FileController(IMediator mediator, ResumableFileUploadService resumableUploads) : ControllerBase
    {
        [HttpGet, AllowAnonymous]
        public async Task<PageResult<FileDto>> SearchFile([FromQuery] FileQuery query, CancellationToken ct)
            => await mediator.Send(query, ct);

        [HttpGet("preview"), AllowAnonymous]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> PreviewFile([FromQuery] PreviewFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);
            return File(result.FileStream, result.ContentType, enableRangeProcessing: true);
        }

        [HttpGet("download")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadFile([FromQuery] DownloadFileQuery query, CancellationToken ct)
        {
            var result = await mediator.Send(query, ct);
            return File(result.FileStream, result.ContentType, result.FileName, enableRangeProcessing: true);
        }

        [HttpPost("Upload"), ServiceFilter(typeof(FileUploadFilter)), RequestFormLimits(ValueLengthLimit = 100 << 10)]
        public async Task<IEnumerable<Guid>> UploadFile([FromForm] CreateFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpPatch, ServiceFilter(typeof(FileUploadFilter)), RequestSizeLimit(100 << 10)]
        public async Task<Guid> UpdateFile([FromForm] UpdateFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        /// <summary>创建视频断点上传任务。</summary>
        [HttpPost("uploads"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<ResumableFileUploadService.UploadSession> StartVideoUpload(
            [FromBody] ResumableFileUploadService.StartRequest request, CancellationToken ct)
            => await resumableUploads.StartAsync(request, ct);

        /// <summary>上传单个视频分片。分片大小上限为 8 MiB，允许通过任务状态查询已成功的分片。</summary>
        [HttpPut("uploads/{uploadId:guid}/chunks/{index:int}"), RequestSizeLimit(9 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 9 * 1024 * 1024)]
        public async Task<IActionResult> UploadVideoChunk(Guid uploadId, int index, IFormFile chunk, CancellationToken ct)
        {
            await resumableUploads.WriteChunkAsync(uploadId, index, chunk, ct);
            return NoContent();
        }

        [HttpGet("uploads/{uploadId:guid}")]
        public async Task<ResumableFileUploadService.UploadStatus> GetVideoUploadStatus(Guid uploadId, CancellationToken ct)
            => await resumableUploads.GetStatusAsync(uploadId, ct);

        /// <summary>校验所有分片并合并，之后交由现有文件管线完成哈希、去重、元数据和存储。</summary>
        [HttpPost("uploads/{uploadId:guid}/complete"), ServiceFilter(typeof(FileUploadFilter))]
        public async Task<IEnumerable<Guid>> CompleteVideoUpload(Guid uploadId, CancellationToken ct)
        {
            var assembled = await resumableUploads.AssembleAsync(uploadId, ct);
            try
            {
                await using var stream = new FileStream(assembled.TempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var formFile = new FormFile(stream, 0, assembled.FileSize, "File", assembled.FileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = Path.GetExtension(assembled.FileName).ToLowerInvariant() == ".webm" ? "video/webm" : "video/mp4"
                };
                return await mediator.Send(new CreateFileCommand(false, formFile), ct);
            }
            finally
            {
                await resumableUploads.DeleteSessionAsync(uploadId);
            }
        }

        [HttpPatch("preview/complete"), AllowAnonymous]
        public async Task<Guid> BlogViewed([FromQuery] BlogPreviewCompleteCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpPatch("tag")]
        public async Task<Guid> UpdateFileTag(UpdateFileTagCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpDelete]
        public async Task DeleteFile(DeleteFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpPatch("CancelSchedule")]
        public async Task<Guid> CancelSchedule(CancelScheduledFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);

        [HttpPatch("schedule")]
        public async Task<Guid> ScheduleFile(ScheduledFileCommand command, CancellationToken ct)
            => await mediator.Send(command, ct);
    }
}
