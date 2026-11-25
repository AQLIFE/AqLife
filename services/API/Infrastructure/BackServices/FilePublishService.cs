using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Infrastructure.BackServices
{
    public sealed class FilePublishService(IFilePersistence persistence) : IFilePublishService
    {
        private readonly IQueryable<FileMetaEntity> fileQueryable = persistence.Files.Include(e => e.PublishMeta);
        public async Task<Guid> PublishAsync(Guid guid, CancellationToken ct = default)
        {
            FileMetaEntity file = await fileQueryable.SingleOrDefaultAsync(e => e.UID == guid, ct) ?? throw new ResourceNotFoundException("File not found");
            file.PublishMeta.Publish();
            //await dbContext.SaveChangesAsync(ct);
            return guid;
        }

        public async Task<int> PublishAsync(IEnumerable<Guid> guids, CancellationToken ct = default)
        {
            List<FileMetaEntity> files = await fileQueryable.Where(f => guids.Contains(f.UID) && f.PublishMeta.PublishStatus != FileStatus.Published).ToListAsync(ct);
            foreach (var file in files)
            {
                file.PublishMeta.Publish();
            }
            return files.Count;
            //return await dbContext.SaveChangesAsync(ct);// 返回发布的数量:对于数量是否一致或者其他要求,交由调用者处理

        }
        public async Task<Guid> Agreement(Guid guid, DateTimeOffset agreementTime, CancellationToken ct = default)
        {
            FileMetaEntity file = await fileQueryable.SingleOrDefaultAsync(e => e.UID == guid, ct) ?? throw new ResourceNotFoundException("File not found");
            file.PublishMeta.Schedule(agreementTime);
            return guid;
        }

        public async Task<Guid> Handle(Guid fileId, DateTimeOffset? dateTimeOffset = null, CancellationToken ct = default)
        {
            if (dateTimeOffset is DateTimeOffset offset) return await Agreement(fileId, offset, ct);
            else return await PublishAsync(fileId, ct);
        }
    }
}
