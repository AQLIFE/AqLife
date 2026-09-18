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
    public class PublishFileHandler(FileWriter fileWriter) : IRequestHandler<PublishFileCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(PublishFileCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command.GetFiles(), publishAt: command.ScheduledTime, ct: ct);
    }

    public class ScheduledBlogHandler(IApplicationDbContext context) : IRequestHandler<ScheduledFileCommand,Guid>
    {
        public async Task<Guid> Handle(ScheduledFileCommand command,CancellationToken ct)
        {
            FileMetaEntity file = await context.File.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的文件");
            if (command.ScheduledAt is DateTimeOffset offset) file.Schedule( offset );
            else file.Publish();
            return command.UID;
        }
    }
}
