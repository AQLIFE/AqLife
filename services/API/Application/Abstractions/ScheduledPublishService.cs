using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.BackServices;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Abstractions
{
    public sealed class FilePublishService(IApplicationDbContext context): IFilePublishService
    {
        public async Task<Guid> PublishAsync(Guid guid,CancellationToken ct)
        {
            FileMetaEntity file = await context.File.FindAsync([guid],ct) ?? throw new ResourceNotFoundException("File not found");
            file.PublishMeta.Publish();
            //await context.SaveChangesAsync(ct);
            return guid;
        }

        public async Task<int> PublishAsync(IEnumerable<Guid> guids, CancellationToken ct)
        {
            List<FileMetaEntity> files = await context.File.Include(e=>e.PublishMeta).Where(f => guids.Contains(f.UID) && f.PublishMeta.PublishStatus != FileStatus.Published).ToListAsync(ct);
            foreach (var file in files)
            {
                file.PublishMeta.Publish();
            }
            return files.Count;
            //return await context.SaveChangesAsync(ct);// 返回发布的数量:对于数量是否一致或者其他要求,交由调用者处理
     
        }
    }
}
