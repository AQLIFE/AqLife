using MyLife.Application.Abstractions.FileStorage;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.Exceptions;

namespace MyLife.Application.Business.File.Service
{
    public class FileDeleter(AppStorage storage, IFileStorage fileStorage)
    {
        public async Task DeleteAsync(IEnumerable<Guid?> guids, CancellationToken ct)
        {
            foreach (var id in guids)
            {
                if (id == Guid.Empty || id == null) continue;
                FileMetaEntity file = await storage.File.FindAsync([id], ct) ?? throw new ResourceNotFoundException("文件不存在");
                await fileStorage.DeleteAsync(file.UID + file.Extension, ct);
                storage.File.Remove(file);
            }
        }
    }
}
