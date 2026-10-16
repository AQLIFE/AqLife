using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.BackServices;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
    /// <summary>
    /// 若没有指定发布时间，则立即发布；若指定了发布时间，则设置为定时发布
    /// </summary>
    /// <param name="context"></param>

    public class ScheduledBlogHandler(IFilePublishService publishService) : IRequestHandler<ScheduledFileCommand,Guid>
    {
        public async Task<Guid> Handle(ScheduledFileCommand command,CancellationToken ct)
        {
            if (command.ScheduledAt is DateTimeOffset offset)return await publishService.Agreement(command.UID, offset, ct);
            else return await publishService.PublishAsync(command.UID,ct);            
        }
    }
}
