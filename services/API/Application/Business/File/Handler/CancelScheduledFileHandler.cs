using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Handler
{
    public class CancelScheduledFileHandler(IApplicationDbContext appStorage) :IRequestHandler<CancelScheduledFileCommand, Guid>
    {
        public async Task<Guid> Handle(CancelScheduledFileCommand command,CancellationToken ct)
        {
            FileMetaEntity entity = await appStorage.File.Include(e=>e.PublishMeta).SingleOrDefaultAsync(e=>e.UID ==command.UID,ct) ?? throw new FileNotFoundException("不存在的文件,无法操作");
            entity.PublishMeta.CancelSchedule();
            return command.UID;
        }
    }
}
