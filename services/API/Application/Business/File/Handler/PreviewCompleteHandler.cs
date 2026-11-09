using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.File.Handler
{
    public class PreviewCompleteHandler(IApplicationDbContext dbContext) : IRequestHandler<BlogPreviewCompleteCommand, Guid>
    {
        public async Task<Guid> Handle(BlogPreviewCompleteCommand command, CancellationToken ct)
        {

            FileMetaEntity fileMeta = await dbContext.File.Include(i => i.InteractionMeta).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new ResourceNotFoundException("不存在的文件");
            fileMeta.InteractionMeta.Viewed();
            return command.UID;
        }
    }
}
