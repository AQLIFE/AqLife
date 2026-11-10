using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Corpus.Search
{
    public class AllCorpusSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<CorpusEntity, EntitySearchCriteria>(httpContext)
    {
        //public override IQueryable<CorpusEntity> Execute(
        //   IQueryable<CorpusEntity> queryable,
        //   EntitySearchCriteria criteria,
        //   CancellationToken ct = default)
        //{
        //    return queryable;
        //}
    }
}
