using MediatR;

using MyLife.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using MyLife.Application.Abstractions.FileStorage;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.Tools;

namespace MyLife.Application.Business.File.Handler
{
    public class UpdateFileHandler(IApplicationDbContext appStorage,IFileStorage fileStorage,UploadContext uploadContext) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await appStorage.File.FindAsync(command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            entity.FileHash = uploadContext.FileHashes.FirstOrDefault(e => e.Key == command.File).Value;
            //string filename = command.UID + Path.GetExtension(command.File.FileName);
            await fileStorage.SaveAsync(command.File.OpenReadStream(),entity.StorageName , ct);
            return command.UID;
        }
    }
}
