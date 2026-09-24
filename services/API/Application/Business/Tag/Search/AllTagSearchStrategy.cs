using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Tag.Search
{
    public sealed class AllTagSearchStrategy
        : AllSearchStrategyBase<TagEntity, EntitySearchCriteria>
    {
        public override async Task<IQueryable<TagEntity>> ExecuteAsync(
            IQueryable<TagEntity> queryable,
            EntitySearchCriteria criteria,
            CancellationToken ct = default)
        {
            return queryable;
        }
    }
}
