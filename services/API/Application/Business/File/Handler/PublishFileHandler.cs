using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    /// <summary>
    /// 上传并设置发布时间
    /// </summary>
    /// <param name="fileWriter"></param>
    public class PublishFileHandler(FileWriter fileWriter) : IRequestHandler<PublishScheduledCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(PublishScheduledCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command.GetFiles(), publishAt: command.ScheduledTime, ct: ct);
    }

    /// <summary>
    /// 手动发布 : 事务依赖,自动保存和回滚
    /// </summary>
    /// <param name="context"></param>
    public class ManualPublishFileHandler(IApplicationDbContext context) : IRequestHandler<PublishFileCommand, Guid>
    {
        public async Task<Guid> Handle(PublishFileCommand command, CancellationToken ct)
        {
            FileMetaEntity file = await context.File.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的文件");
            file.Publish();
            return file.UID;
        }
    }

    public class ScheduledBlogHandler(IApplicationDbContext context) : IRequestHandler<ScheduledBlogCommand,Guid>
    {
        public async Task<Guid> Handle(ScheduledBlogCommand command,CancellationToken ct)
        {
            FileMetaEntity file = await context.File.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的文件");
            file.Schedule(command.ScheduledAt);
            return command.UID;
        }
    }
}
