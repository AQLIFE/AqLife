using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.File;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Options;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AqLife.Application.File.Validator;

/// <summary>
/// 不允许更新非 MD 文件
/// </summary>
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
public class FileNameConsistencyValidtor(ISearch<FileQuery, PageResult<FileDto>, FileMetaEntity> search)
    : AbstractValidator<UpdateFileCommand>
{
    protected override string ErrorMessage { init; get; } = "更新文件名必须与原文件一致";
    protected override async Task<bool> IsValidAsync(UpdateFileCommand command, CancellationToken ct)
    {
        var fileCache = await search.SearchAsync(new FileQuery(UID: command.UID, Scope: FileScope.All), ct);
        return command.File.FileName == (fileCache.First().FileName + fileCache.First().Extension);
    }
}

/// <summary>
/// 预期标签不存在
/// </summary>
public class UpdateFileTagValidator(IApplicationDbContext context) : AbstractValidator<UpdateFileTagCommand>
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
public class UpdateFileCategoryValidator(IApplicationDbContext context) : AbstractValidator<UpdateFileCategoryCommand>
{
    protected override string ErrorMessage { init; get; } = "预期分类不存在";
    protected override async Task<bool> IsValidAsync(UpdateFileCategoryCommand command, CancellationToken ct)
        => await context.Categories.AnyAsync(e => e.UID == command.categoryId, ct);
}
