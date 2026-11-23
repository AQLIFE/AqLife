using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Queries;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;
using AqLife.Application.Tag;

namespace AqLife.Application.Tag.Search
{
    public sealed class TagSearch(ISearchMapper<TagQuery, PageResult<TagDto>, EntitySearchCriteria> queryMapper, ITagQuerySource querySource, IHttpContextAccessor httpContext,
     IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, PageResult<TagDto>, TagEntity, EntitySearchCriteria>(httpContext, searchStrategies)
    {
        protected override IQueryable<TagEntity> GetQuery() => querySource.Query();

        protected override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<TagEntity> BuildBaseQuery(IQueryable<TagEntity> queryable, TagQuery query)
       => queryable;

        protected override IQueryable<TagEntity> ApplyDefaultOrder(IQueryable<TagEntity> queryble, TagQuery query)
        => queryble.OrderBy(e => e.Name);
    }
}
