using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqLife.Application.Business.Corpus.Search
{
    public class AllCorpusSearchStrategy:AllSearchStrategyBase<CorpusEntity, EntitySearchCriteria>
    {
        public override async Task<IEnumerable<CorpusEntity>> ExecuteAsync(
           IQueryable<CorpusEntity> queryable,
           EntitySearchCriteria criteria,
           CancellationToken ct = default)
        {
            return await queryable.ToListAsync(ct);
        }
    }
}
