using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities.File;
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
    : AllSearchStrategyBase<FileMetaEntity, FileSearchCriteria>
    {
        public override bool IsMatch(FileSearchCriteria c)
        => base.IsMatch(c) && c.CategoryUID is null;
        public override async Task<IQueryable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            FileSearchCriteria criteria,
            CancellationToken ct = default)
        {
            return httpContext.HttpContext?.User.Identity?.IsAuthenticated != true?
                queryable.Where(e => e.Extension.Contains(".md")):
                queryable;
            
        }
    }
}
