using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;

namespace Corpus.Search
{
    public sealed class FilteredCorpusSearchStrategy : FilteredSearchStrategyBase<CorpusEntity, EntitySearchCriteria>
    {
        protected override IQueryable<CorpusEntity> ApplyKeywordFilter(IQueryable<CorpusEntity> q, string keyword)
        => q.Where(e => e.CorpusContent.Contains(keyword));
    }
}
