using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Options;
using Core.Validators;
using File;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace File.Validator;

/// <summary>
/// 不允许更新非 MD 文件
/// </summary>
/// <param name="options"></param>
public class FileUpdateValidtor(IOptions<FilePolicyOption> options) : AbstractValidator<UpdateFileCommand>
{
    protected override string ErrorMessage { init; get; } = "不允许更新非 MD 文件";
    protected override async Task<bool> IsValidAsync(UpdateFileCommand command, CancellationToken ct)
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

/// <summary>
/// 没有进行过上传的文件不可以用于更新
/// </summary>
/// <param name="search"></param>
public class FileNameConsistencyValidtor(ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity> search/*,ILogger<FileNameConsistencyValidtor> logger*/) : AbstractValidator<UpdateFileCommand>
{
    protected override string ErrorMessage { init; get; } = "更新文件名必须与原文件一致";
    protected override async Task<bool> IsValidAsync(UpdateFileCommand command, CancellationToken ct)
    {
        var fileCache = await search.SearchAsync(new FileQuery(UID: command.UID, Scope: FileScope.All), ct);
        //logger.LogInformation($"更新文件名必须与原文件一致{command.File.Name},{command.File.FileName},{fileCache.First().FileName}");
        return command.File.FileName == (fileCache.First().FileName + fileCache.First().Extension);
    }
}

/// <summary>
/// 预期发布时间已过期
/// </summary>
/// <param name="search"></param>
public class FilePublishStatusValidtor(ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity> search) : AbstractValidator<ScheduledFileCommand>
{
    protected override string ErrorMessage { init; get; } = "预定时间不能小于当前系统时间";
    protected override async Task<bool> IsValidAsync(ScheduledFileCommand command, CancellationToken ct)
    {
        var fileCache = await search.SearchAsync(new FileQuery(command.UID, Scope: FileScope.All), ct);// 发布文件,不对文件类型做区分,这样可以允许模板文件发布
        return !(fileCache.First().PublishMeta.PublishStatus == FileStatus.Published && command.ScheduledAt < DateTimeOffset.UtcNow);
        // 考虑到 command.ScheduledAt 可以为null ,当它等于null时,这个表达式通过
        // 若不等于null,则参与运算,小于当前系统时间则拦截,反之通过
    }
}
/// <summary>
/// 预期标签不存在
/// </summary>
/// <param name="context"></param>
public class UpdateFileTagValidator(IApplicationDbContext context): AbstractValidator<UpdateFileTagCommand>
{
    protected override string ErrorMessage { init; get; } = "一个或多个关联标签不存在";

    protected override async Task<bool> IsValidAsync(UpdateFileTagCommand command, CancellationToken ct)
    {
        if (!command.tags.Any())
            return true;
        return command.tags.Where(id => id != Guid.Empty).Distinct().All(e => context.Tags.Any(tag => tag.UID == e));
    }
}

/// <summary>
/// 预期分类不存在
/// </summary>
/// <param name="context"></param>
public class UpdateFileCategoryValidator(IApplicationDbContext context) : AbstractValidator<UpdateFileCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "预期分类不存在";

    protected override async Task<bool> IsValidAsync(UpdateFileCategoryCommand command, CancellationToken ct)
    => await context.Categories.AnyAsync(e => e.UID == command.categoryId,ct);
}