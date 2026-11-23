using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;

namespace AqLife.Application.File.Service;

public class FileDeleter(IFilePersistence persistence, UploadContext uploadContext)
{
    public async Task DeleteAsync(IEnumerable<Guid> guids, CancellationToken ct)
    {
        foreach (var id in guids)
        {
            if (id == Guid.Empty) continue;
            FileMetaEntity file = await persistence.FindAsync(id, ct)
                ?? throw new ResourceNotFoundException("文件不存在");
            persistence.RemoveFile(file);
            uploadContext.DeletePath.Add(file.StorageKey);
        }
    }
}
