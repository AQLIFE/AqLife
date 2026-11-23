using AqLife.Application.Abstractions.Contracts.File.Storage;
using AqLife.Application.Abstractions.Persistence;
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
    public class ScheduledBlogHandler(IFilePublishService publishService) : IRequestHandler<ScheduledFileCommand, Guid>
    {
        public async Task<Guid> Handle(ScheduledFileCommand command, CancellationToken ct)
        => await publishService.Handle(command.UID, command.ScheduledAt, ct);
    }
    /// <summary>
    /// 取消发布
    /// </summary>
    /// <param name="dbContext"></param>
    public class CancelScheduledFileHandler(IApplicationDbContext dbContext) : IRequestHandler<CancelScheduledFileCommand, Guid>
    {
        public async Task<Guid> Handle(CancelScheduledFileCommand command, CancellationToken ct)
        {
            FileMetaEntity entity = await dbContext.File.Include(e => e.PublishMeta).SingleOrDefaultAsync(e => e.UID == command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法操作");
            entity.PublishMeta.CancelSchedule();
            return command.UID;
        }
    }
    /// <summary>
    /// 阅览完成
    /// </summary>
    /// <param name="dbContext"></param>
}
