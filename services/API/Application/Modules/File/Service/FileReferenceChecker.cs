using AqLife.Application.Abstractions.Contracts.File;

namespace AqLife.Application.File.Service;

/// <summary>
/// File 模块统一的跨模块引用检查入口。
/// </summary>
public sealed class FileReferenceChecker(
    IEnumerable<IFileReferenceProvider> referenceProviders)
{
    public async Task<bool> IsReferencedAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        foreach (var provider in referenceProviders)
        {
            if (await provider.IsReferencedAsync(fileId, cancellationToken))
                return true;
        }

        return false;
    }
}
