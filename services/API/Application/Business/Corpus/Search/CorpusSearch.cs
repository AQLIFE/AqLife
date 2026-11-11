using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Corpus.Search
{
    public sealed class CorpusSearch(QueryMapper queryMapper, IApplicationDbContext storage, IHttpContextAccessor httpContext, IEnumerable<ISearchStrategy<CorpusEntity, EntitySearchCriteria>> searchStrategies)
        : BaseSearch<CorpusQuery, CorpusEntity, CorpusDto, EntitySearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected private override IQueryable<CorpusEntity> BuildBaseQuery(IQueryable<CorpusEntity> queryable, CorpusQuery query)
        => queryable;

        protected private override EntitySearchCriteria MapToCriteria(CorpusQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<CorpusEntity> ApplyDefaultOrder(IQueryable<CorpusEntity> queryble, CorpusQuery query)
        => queryble.OrderBy(e => e.CreateDate);
    }
}
