using AqLife.Application.Search;
using AqLife.Domain.Entities;

namespace AqLife.Application.Business.Corpus.Search
{
    public class AllCorpusSearchStrategy : AllSearchStrategyBase<CorpusEntity, EntitySearchCriteria>
    {
        public override IQueryable<CorpusEntity> Execute(
           IQueryable<CorpusEntity> queryable,
           EntitySearchCriteria criteria,
           CancellationToken ct = default)
        {
            return queryable;
        }
    }
}
