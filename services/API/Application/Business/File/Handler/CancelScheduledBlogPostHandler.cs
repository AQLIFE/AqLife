using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Handler
{
    public class CancelScheduledBlogPostHandler(IApplicationDbContext appStorage) :IRequestHandler<CancelScheduledBlogPostCommand, Guid>
    {
        public async Task<Guid> Handle(CancelScheduledBlogPostCommand command,CancellationToken ct)
        {
            FileMetaEntity entity = await appStorage.File.FindAsync(command.UID, ct) ?? throw new FileNotFoundException("不存在的文件,无法操作");
            entity.CancelSchedule();
            return command.UID;
        }
    }
}
