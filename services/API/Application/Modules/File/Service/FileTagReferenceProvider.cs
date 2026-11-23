using AqLife.Application.Abstractions.Contracts.Tag;
using AqLife.Application.Abstractions.Persistence.Modules;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.File.Service;

/// <summary>
/// File 模块拥有的 Tag 引用检查。
/// </summary>
public sealed class FileTagReferenceProvider(IFilePersistence persistence) : ITagReferenceProvider
{
    public Task<bool> IsReferencedAsync(Guid tagId, CancellationToken cancellationToken = default)
        => persistence.Files.SelectMany(file => file.FileTags)
            .AnyAsync(fileTag => fileTag.TagId == tagId, cancellationToken);
}
