using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Abstractions.Contracts.File;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.File;

namespace AqLife.Application.File.Handler
{
    /// <summary>
    /// 更新文件内容
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="fileStorage"></param>
    /// <param name="uploadContext"></param>
    public class PreviewCompleteHandler(IFilePersistence persistence) : IRequestHandler<BlogPreviewCompleteCommand, Guid>
    {
        public async Task<Guid> Handle(BlogPreviewCompleteCommand command, CancellationToken ct)
        {

            FileMetaEntity fileMeta = await persistence.Files.Include(i => i.InteractionMeta).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("不存在的文件");
            fileMeta.InteractionMeta.Viewed();
            return command.UID;
        }
    }
}
