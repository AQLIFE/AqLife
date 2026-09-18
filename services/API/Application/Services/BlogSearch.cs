using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Services
{
    public class BlogSearch(IApplicationDbContext context)
    {
        public async Task<IEnumerable<FileMetaEntity>> GetScheduledPostsDueAsync(DateTimeOffset ScheduledTime,CancellationToken ct)
        => await context.File.Where(e => e.PublishStatus == FileStatus.Scheduled && e.PublishAt <= ScheduledTime).ToListAsync(ct);

    }
}
