using AqLife.Application.BackServices;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using MediatR;

namespace AqLife.Application.Business.File.Handler
{
    /// <summary>
    /// 若没有指定发布时间，则立即发布；若指定了发布时间，则设置为定时发布
    /// </summary>
    /// <param name="context"></param>

    public class ScheduledBlogHandler(IFilePublishService publishService) : IRequestHandler<ScheduledFileCommand, Guid>
    {
        public async Task<Guid> Handle(ScheduledFileCommand command, CancellationToken ct)
        =>await publishService.Handle(command.UID, command.ScheduledAt, ct);
    }
}
