using AqLife.Application.File.Service;
using Core.Validators;

namespace AqLife.Application.File.Validator;

/// <summary>
/// 不允许删除仍被其他业务模块引用的文件。
/// 每个业务模块通过 IFileReferenceProvider 自主检查自己的引用关系。
/// </summary>
public sealed class FileDeleteValidtor(FileReferenceChecker referenceChecker)
    : AbstractValidator<DeleteFileCommand>
{
    protected override string ErrorMessage { init; get; } =
        "预期删除的文件被其他业务依赖着,不允许删除";

    protected override Task<bool> IsValidAsync(
        DeleteFileCommand command,
        CancellationToken ct)
        => referenceChecker.IsReferencedAsync(command.UID, ct);
}
