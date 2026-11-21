using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Modules.Corpus.Search
{
    public sealed class CorpusSearch(ISearchMapper<CorpusQuery, PageResult<CorpusDto>, EntitySearchCriteria> queryMapper, IApplicationDbContext dbContext, IHttpContextAccessor httpContext, IEnumerable<ISearchStrategy<CorpusEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<CorpusQuery, PageResult<CorpusDto>, CorpusEntity, EntitySearchCriteria>(dbContext, httpContext, searchStrategies)
    {
        protected override IQueryable<CorpusEntity> BuildBaseQuery(IQueryable<CorpusEntity> queryable, CorpusQuery query)
        => queryable;

        protected override EntitySearchCriteria MapToCriteria(CorpusQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<CorpusEntity> ApplyDefaultOrder(IQueryable<CorpusEntity> queryble, CorpusQuery query)
        => queryble.OrderBy(e => e.CreateDate);
    }
}
