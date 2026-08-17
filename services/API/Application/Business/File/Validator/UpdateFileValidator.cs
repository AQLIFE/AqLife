using Microsoft.Extensions.Options;
using MyLife.Application.Validators;
using MyLife.Domain.Command;
using MyLife.Shared.Options;

namespace MyLife.Application.Business.File.Validator;



/// <summary>
/// 不允许更新非 MD 文件
/// </summary>
/// <param name="options"></param>
public class FileUploadValidtor(IOptions<FilePolicyOption> options) : AbstractValidator<UpdateFileCommand>
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