using MyLife.Application.Abstractions.FileStorage;
using MyLife.Domain.Entities;
using MyLife.Shared.Exceptions;
using MyLife.Application.Abstractions.Persistence;
namespace MyLife.Application.Business.File.Service
{
    public class FileDeleter(IApplicationDbContext storage, IFileStorage fileStorage)
    {
        public async Task DeleteAsync(IEnumerable<Guid> guids, CancellationToken ct)
        {
            foreach (var id in guids)
            {
                if (id == Guid.Empty ) continue;
                FileMetaEntity file = await storage.File.FindAsync([id], ct) ?? throw new ResourceNotFoundException("文件不存在");
                await fileStorage.DeleteAsync(file.StorageName, ct);
                storage.File.Remove(file);
            }
        }
    }
}
