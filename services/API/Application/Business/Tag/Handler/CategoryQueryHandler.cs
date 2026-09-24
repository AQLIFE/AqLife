using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Tag.Handler
{
    public class CategoryQueryHandler(IApplicationDbContext dbContext,IOptions<FilePolicyOption> options) : IRequestHandler<CategoryQuery, IEnumerable<BlogCategoryStatistics>?>
    {
        public async Task<IEnumerable<BlogCategoryStatistics>?> Handle(CategoryQuery query, CancellationToken ct)
        {
            // allowedDownload 目前只有 .md
            return await dbContext.Tags.Include(e=>e.FileTags).ThenInclude(x=>x.File).Where(e => e.IsCategory)
                .Select(e => new BlogCategoryStatistics( e.UID.ToString(),  e.Name, e.FileTags.Count(ft => ft.File.PublishMeta.PublishStatus == FileStatus.Published && options.Value.AllowedDownload.Contains(ft.File.Extension)))).ToListAsync(ct);

        }
    }
}
