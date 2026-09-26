using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AqLife.Application.Business.File.Handler
{
    public class PreviewCompleteHandler(IApplicationDbContext dbContext) : IRequestHandler<BlogPreviewCompleteCommand, Guid>
    {
        public async Task<Guid> Handle(BlogPreviewCompleteCommand command, CancellationToken ct)
        {

            FileMetaEntity fileMeta = await dbContext.File.Include(i=>i.InteractionMeta).SingleOrDefaultAsync(e=>e.UID==command.UID,ct) ?? throw new ResourceNotFoundException("不存在的文件");
            fileMeta.InteractionMeta.Viewed();
            return command.UID;
        }
    }
}
