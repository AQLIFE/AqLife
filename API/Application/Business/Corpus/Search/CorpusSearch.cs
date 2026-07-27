using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mapper;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Corpus.Search
{
    public class CorpusSearch(QueryMapper queryMapper, AppStorage storage, IEnumerable<ISearchStrategy<CorpusEntity, EntitySearchCriteria>> searchStrategies) : BaseSearch<CorpusQuery, CorpusEntity, CorpusDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override async Task<IQueryable<CorpusEntity>> BuildBaseQueryAsync(IQueryable<CorpusEntity> queryable, CorpusQuery query)
        => queryable;

        protected override EntitySearchCriteria MapToCriteria(CorpusQuery query)
        => queryMapper.ToCriteria(query);
    }
}
