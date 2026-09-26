using AqLife.Application.Abstractions.Search;
using AqLife.Application.Business.File.Search;
using AqLife.Application.Business.File.Service;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AqLife.Application.Business.File.Handler
{
    public class PreviewCompleteHandler(FileSearch fileSearch) : IRequestHandler<BlogPreviewCompleteCommand, Guid>
    {
        public async Task<Guid> Handle(BlogPreviewCompleteCommand command, CancellationToken ct)
        {
            
            IQueryable<FileMetaEntity> file = await fileSearch.SearchAsync(new FileQuery(UID: command.UID), ct) ?? throw new ResourceNotFoundException("不存在的文件");
            if (file.FirstOrDefault() is FileMetaEntity fileMeta)
            {
                fileMeta.InteractionMeta.Viewed();
            }
            return command.UID;
        }
    }
}
