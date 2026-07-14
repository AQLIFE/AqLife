using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyLife.Application.Command;
using MyLife.Data.Repository;
using MyLife.Shared.Command;
using MyLife.Shared.Options;
using MyLife.Shared.Tools;

namespace MyLife.Application.Validators.BusinessValidator;

public class PreviewFileValidator(AppStorage storage) : AbstractValidator<PreviewFileQuery>
{
    private protected override string ErrorMessage { init; get; } = "不支持预览的文件类型";
    private protected override async Task<bool> IsValidAsync(PreviewFileQuery command, CancellationToken ct)
    {
        var entity = await storage.File.AsNoTracking().FirstAsync(e => e.UID == command.UID);
        return entity.Extension !=".md";// 存在 另一个前置ID检查，此时 不应为NULL    
    }
}

/// <summary>
/// 文件类型上传策略,适用创建和更新
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
public class FileSizeValidator<T>(IOptions<FilePolicyOption> options) : AbstractValidator<T> where T : IHasFormFiles
{
    private protected override string ErrorMessage { init; get; } = $"文件大小超出限制 (最大允许: {options.Value.MaxFileSize}";
    private protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
    {
        long limit = (long)options.Value.MaxFileSize << options.Value.StorageUnit;
        // 等于 2^ StorageUnit * MaxFileSize
        // dev 配置设置到100 KB=> 100 * 2^10 字节

        return command.GetFiles().All(e => e.Length <= limit);
    }
}


/// <summary>
/// 异步策略1 : 检查文件是否重复（通过计算文件的哈希值并与数据库中已有文件的哈希值进行比较）
/// </summary>
/// <param name="storage"></param>
/// <param name="context"></param>
public class FileDuplicateValidator<T>(UploadContext context) : AbstractValidator<T> where T : IHasFormFiles
{
    private protected override string ErrorMessage { init; get; } = "文件重复";
    private protected override async Task<bool> IsValidAsync(T command, CancellationToken ct)
    {
        var files = command.GetFiles();

        foreach (var file in files)
        {
            var tempHash = await CalculateHashAsync(file);
            //if (await storage.File.AsNoTracking().AnyAsync(e => e.FileHash == tempHash)) return true;
            context.FileHashes.Add(file, tempHash);
        }
        return true;

        // 文件不存在[也会存在新HASH] 和 文件存在但哈希不同 都算有效
        // 对于文件存在但hash不同,属于update,后续业务需要做合并或者更新处理
    }
    private static async Task<string> CalculateHashAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
