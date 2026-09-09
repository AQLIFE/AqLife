using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    public class PublishFileHandler(FileWriter fileWriter) : IRequestHandler<PublishScheduledCommand, IEnumerable<Guid>>
    {
        public async Task<IEnumerable<Guid>> Handle(PublishScheduledCommand command, CancellationToken ct)
        => await fileWriter.WriteAsync(command.GetFiles(), publishAt: command.ScheduledTime, ct: ct);
    }


    public class ManualPublishFileHandler(IApplicationDbContext context) : IRequestHandler<PublishFileCommand, Guid>
    {
        public async Task<Guid> Handle(PublishFileCommand command, CancellationToken ct)
        {
            FileMetaEntity file = await context.File.FindAsync([command.UID], ct) ?? throw new ResourceNotFoundException("不存在的文件");
            file.Publish();
            return file.UID;
        }
    }
}
