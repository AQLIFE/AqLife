using Core.Search;
using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Modules.Tag.Search
{
    public sealed class TagSearch(ISearchMapper<TagQuery, PageResult<TagDto>, EntitySearchCriteria> queryMapper, IApplicationDbContext storage, IHttpContextAccessor httpContext,
     IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, PageResult<TagDto>, TagEntity, EntitySearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected override IQueryable<TagEntity> BuildBaseQuery(IQueryable<TagEntity> queryable, TagQuery query)
       => queryable;

        protected override IQueryable<TagEntity> ApplyDefaultOrder(IQueryable<TagEntity> queryble, TagQuery query)
        => queryble.OrderBy(e => e.Name);
    }
}
