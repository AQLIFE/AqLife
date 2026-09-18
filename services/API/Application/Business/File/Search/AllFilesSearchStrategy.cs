using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Contracts;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.File.Search
{
    public sealed class AllFilesSearchStrategy
    : AllSearchStrategyBase<FileMetaEntity, EntitySearchCriteria>
    {
        public override async Task<IEnumerable<FileMetaEntity>> ExecuteAsync(
            IQueryable<FileMetaEntity> queryable,
            EntitySearchCriteria criteria,
            CancellationToken ct = default)
        {
            queryable = queryable.Where(e =>e.Extension.Contains(".md"));

            return await queryable.ToListAsync(ct);
        }
    }
}
