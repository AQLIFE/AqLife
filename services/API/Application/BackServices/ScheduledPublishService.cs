using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.File;
using AqLife.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Services
{
    public sealed class BlogPublishService(IApplicationDbContext context): IBlogPublishService
    {
        public async Task<Guid> PublishAsync(Guid guid,CancellationToken ct)
        {
            FileMetaEntity blog = await context.File.FindAsync([guid],ct) ?? throw new ResourceNotFoundException("File not found");
            blog.PublishMeta.Publish();
            await context.SaveChangesAsync(ct);
            return guid;
        }
    }
}
