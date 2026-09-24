using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Handler
{
    public class UpdateFileHandler(IApplicationDbContext appStorage, IFileStorage fileStorage, UploadContext uploadContext) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await appStorage.File.Include(e => e.PublishMeta).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            string hash = uploadContext.FileHashes[command.File];
            _ = entity.UpdateHash(hash);
            await fileStorage.SaveAsync(command.File.OpenReadStream(), entity.StorageKey, ct);// 覆写到原来的文件
            return command.UID;
        }
    }
}
