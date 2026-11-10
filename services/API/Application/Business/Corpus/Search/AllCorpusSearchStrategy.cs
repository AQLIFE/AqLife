using AqLife.Application.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Corpus.Search
{
    public sealed class AllCorpusSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<CorpusEntity, EntitySearchCriteria>(httpContext)
    {
    }
}
