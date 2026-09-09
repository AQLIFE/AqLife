using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Application.Mappers;

namespace AqLife.Application.Business.Corpus.Search
{
    public class CorpusSearch(QueryMapper queryMapper, IApplicationDbContext storage, IEnumerable<ISearchStrategy<CorpusEntity, EntitySearchCriteria>> searchStrategies) : BaseSearch<CorpusQuery, CorpusEntity, CorpusDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override async Task<IQueryable<CorpusEntity>> BuildBaseQueryAsync(IQueryable<CorpusEntity> queryable, CorpusQuery query)
        => queryable;

        protected override EntitySearchCriteria MapToCriteria(CorpusQuery query)
        => queryMapper.ToCriteria(query);
    }
}
