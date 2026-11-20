using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Abstractions.Commands;
using AqLife.Shared.Options;
using Core.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Modules.File.Validator
{

    /// <summary>
    /// 文件类型上传策略
    /// </summary>
    /// <param name="options"></param>
    public class FileTypeValidator<T>(IOptions<FilePolicyOption> options) : AbstractValidator<T> where T : IHasFormFiles
    {
        protected override string ErrorMessage { init; get; } = "不允许上传的文件类型";
        protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
            => command.GetFiles().All(e => options.Value.AllowedUpload.Contains(Path.GetExtension(e.FileName).ToLowerInvariant()));
    }
    /// <summary>
    /// 文件上传策略：普通文件和视频使用独立大小上限
    /// </summary>
    /// <param name="options"></param>
    public class FileSizeValidator<T>(IOptions<FilePolicyOption> options, ILogger<FileSizeValidator<T>> logger) : AbstractValidator<T> where T : IHasFormFiles
    {
        protected override string ErrorMessage { init; get; } = "文件大小超出对应类型的上传限制";
        protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
        {
            var policy = options.Value;
            long regularLimit = options.Value.DefaultFileSize;
            long videoLimit = options.Value.VideoFileSize;

            var files = command.GetFiles().ToList();
            logger.LogDebug("Checking file size limits: regular={RegularLimit} bytes, video={VideoLimit} bytes",
                regularLimit, videoLimit);

            return files.All(file =>
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var isVideo = policy.AllowedVideoExtensions.Contains(extension);
                var limit = isVideo ? videoLimit : regularLimit;
                return file.Length > 0 && limit > 0 && file.Length <= limit;
            });
        }
    }

    /// <summary>
    ///  检查文件是否重复（通过计算文件的哈希值并与数据库中已有文件的哈希值进行比较）
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="context"></param>
    public class FileDuplicateValidator<T> : AbstractValidator<T> where T : IHasFormFiles
    {
        protected override string ErrorMessage { init; get; } = "文件重复";
        protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
        {
            var files = command.GetFiles();
            var keys = files.Select(f => (Name: f.FileName.Trim().ToLowerInvariant(), f.Length)).ToList();
            return keys.Count == keys.Distinct().Count();
        }

    }

    public class FileTemplateValidator(IOptions<FilePolicyOption> options, ILogger<FileTemplateValidator> logger) : AbstractValidator<CreateFileCommand>
    {
        protected override string ErrorMessage { init; get; } = "非 md 文件,禁止标记为模板文件";
        protected override async Task<bool> IsValidAsync(CreateFileCommand command, CancellationToken ct)
        {
            logger.LogDebug("Checking file template validation for command: {Command}", command);
            return command.GetFiles().All(e =>
                options.Value.AllowedBlogExtensions.Contains(Path.GetExtension(e.FileName).ToLowerInvariant()) || !command.IsTemplate
            );
        }
    }

    public class FileListValidator : AbstractValidator<UploadFilesCommand>
    {
        protected override string ErrorMessage { init; get; } = "禁止空上传";
        protected override async Task<bool> IsValidAsync(UploadFilesCommand command, CancellationToken ct)
        => command.GetFiles().Count() > 0;
    }
}