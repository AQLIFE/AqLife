using Core.Search;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Modules.Corpus.Search
{
    public sealed class AllCorpusSearchStrategy(IHttpContextAccessor httpContext) : AllSearchStrategyBase<CorpusEntity, EntitySearchCriteria>(httpContext)
    {
    }
}
