using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Corpus.Search
{
    public sealed class FilteredCorpusSearchStrategy : FilteredSearchStrategyBase<CorpusEntity, EntitySearchCriteria>
    {
        protected private override IQueryable<CorpusEntity> ApplyKeywordFilter(IQueryable<CorpusEntity> q, string keyword)
        => q.Where(e => e.CorpusContent.Contains(keyword));
    }
}
