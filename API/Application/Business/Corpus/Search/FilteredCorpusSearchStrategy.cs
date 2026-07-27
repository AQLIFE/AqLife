using MyLife.Application.Search;
using MyLife.Domain.Entities;

namespace MyLife.Application.Business.Corpus.Search
{
    public class FilteredCorpusSearchStrategy : FilteredSearchStrategyBase<CorpusEntity, EntitySearchCriteria>
    {
        protected override IQueryable<CorpusEntity> ApplyKeywordFilter(
        IQueryable<CorpusEntity> q, string keyword)
        => q.Where(e => e.CorpusContent.Contains(keyword));
    }
}
