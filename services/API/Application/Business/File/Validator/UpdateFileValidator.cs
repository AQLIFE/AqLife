using AqLife.Application.Business.File.Search;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AqLife.Application.Business.File.Validator;



/// <summary>
/// 不允许更新非 MD 文件
/// </summary>
/// <param name="options"></param>
public class FileUpdateValidtor(IOptions<FilePolicyOption> options) : AbstractValidator<UpdateFileCommand>
{
    private protected override string ErrorMessage { init; get; } = "不允许更新非 MD 文件";
    private protected override async Task<bool> IsValidAsync(UpdateFileCommand command, CancellationToken ct)
    {
        var files = command.GetFiles();

        foreach (var file in files)
        {
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!options.Value.AllowedDownload.Contains(ext)) return false;
        }
        return true;
    }
}


public class FileNameConsistencyValidtor(FileSearch search,ILogger<FileNameConsistencyValidtor> logger) : AbstractValidator<UpdateFileCommand>
{
    private protected override string ErrorMessage { init; get; } = "更新文件名必须与原文件一致";
    private protected override async Task<bool> IsValidAsync(UpdateFileCommand command, CancellationToken ct)
    {
        var fileCache = await search.SearchAsync(new FileQuery(command.UID), ct);
        //logger.LogInformation($"更新文件名必须与原文件一致{command.File.Name},{command.File.FileName},{fileCache.First().FileName}");
        return command.File.FileName == (fileCache.First().FileName+fileCache.First().Extension);
    }
}



public class FilePublishStatusValidtor(FileSearch search) : AbstractValidator<ScheduledFileCommand>
{
    private protected override string ErrorMessage { init; get; } = "预定时间不能小于当前系统时间";
    private protected override async Task<bool> IsValidAsync(ScheduledFileCommand command, CancellationToken ct)
    {
        var fileCache = await search.SearchAsync(new FileQuery(command.UID), ct);
        return !(fileCache.First().PublishStatus == FileStatus.Published && command.ScheduledAt < DateTimeOffset.UtcNow);
        // 考虑到 command.ScheduledAt 可以为null ,当它等于null时,这个表达式通过
        // 若不等于null,则参与运算,小于当前系统时间则拦截,反之通过
    }
}