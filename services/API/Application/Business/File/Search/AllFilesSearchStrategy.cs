using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Search
{
    public sealed class AllFilesSearchStrategy(IHttpContextAccessor httpContext)
    : AllSearchStrategyBase<FileMetaEntity, EntitySearchCriteria>
    {
        public override async Task<IEnumerable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            EntitySearchCriteria criteria,
            CancellationToken ct = default)
        {
            if(httpContext.HttpContext?.User.Identity?.IsAuthenticated!=true)
            queryable = queryable.OrderByDescending(e => e.UploadTime).Where(e =>e.Extension.Contains(".md"));// 以发布时间排序，优先显示已发布的文件,默认最新

            return await queryable.ToListAsync(ct);
        }
    }
}
