using AqLife.Application.Abstractions.FileStorage;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Tools;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class UpdateFileHandler(IApplicationDbContext appStorage, IFileStorage fileStorage, UploadContext uploadContext) : IRequestHandler<UpdateFileCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await appStorage.File.FindAsync(command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法更新");
            string hash = uploadContext.FileHashes.FirstOrDefault(e => e.Key == command.File).Value;
            entity.UpdateHash(hash);
            await fileStorage.SaveAsync(command.File.OpenReadStream(), entity.StorageName, ct);
            return command.UID;
        }
    }
}
