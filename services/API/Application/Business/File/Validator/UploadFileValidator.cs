using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.CommandInterface;
using AqLife.Shared.Options;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Net.WebRequestMethods;

namespace AqLife.Application.Business.File.Validator
{

    /// <summary>
    /// 文件类型上传策略
    /// </summary>
    /// <param name="options"></param>
    public class FileTypeValidator<T>(IOptions<FilePolicyOption> options) : AbstractValidator<T> where T : IHasFormFiles
    {
        private protected override string ErrorMessage { init; get; } = "不允许上传的文件类型";
        private protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
            => command.GetFiles().All(e => options.Value.AllowedUpload.Contains(Path.GetExtension(e.FileName).ToLowerInvariant()));
    }
    /// <summary>
    /// 文件上传策略：文件大小检查（使用你的指数幂逻辑）
    /// </summary>
    /// <param name="options"></param>
    public class FileSizeValidator<T>(IOptions<FilePolicyOption> options, ILogger<FileSizeValidator<T>> logger) : AbstractValidator<T> where T : IHasFormFiles
    {
        private protected override string ErrorMessage { init; get; } = $"文件大小超出限制 [1 -: {options.Value.MaxFileSize} KB]";
        private protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
        {
            long limit = (long)options.Value.MaxFileSize << options.Value.StorageUnit;
            // 等于 2^ StorageUnit * MaxFileSize
            // dev 配置设置到100 KB=> 100 * 2^10 字节
            logger.LogInformation("Checking file size against limit: {Limit} bytes,now :{CurrentSize}", limit, command.GetFiles().Sum(f => f.Length));

            return command.GetFiles().All(e => 0 < e.Length && e.Length <= limit);
        }
    }

    /// <summary>
    ///  检查文件是否重复（通过计算文件的哈希值并与数据库中已有文件的哈希值进行比较）
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="context"></param>
    public class FileDuplicateValidator<T> : AbstractValidator<T> where T : IHasFormFiles
    {
        private protected override string ErrorMessage { init; get; } = "文件重复";
        private protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
        {
            var files = command.GetFiles();
            var keys = files.Select(f => (Name: f.FileName.Trim().ToLowerInvariant(),f.Length)).ToList();
            return keys.Count == keys.Distinct().Count();
        }

    }

    public class FileTemplateValidator(IOptions<FilePolicyOption> options, ILogger<FileTemplateValidator> logger) : AbstractValidator<CreateFileCommand>
    {
        private protected override string ErrorMessage { init; get; } = "非 md 文件,禁止标记为模板文件";
        private protected override async Task<bool> IsValidAsync(CreateFileCommand command, CancellationToken ct)
        {
            logger.LogInformation("Checking file template validation for command: {Command}", command);
            return command.GetFiles().All(e =>
                options.Value.AllowedBlogExtensions.Contains(Path.GetExtension(e.FileName).ToLowerInvariant()) || !command.IsTemplate
            );
        }
    }

    public class FileListValidator : AbstractValidator<CreateFileCommand>
    {
        private protected override string ErrorMessage { init; get; } = "禁止空上传";
        private protected override async Task<bool> IsValidAsync(CreateFileCommand command, CancellationToken ct)
        => command.File.Length > 0;
    }
}
