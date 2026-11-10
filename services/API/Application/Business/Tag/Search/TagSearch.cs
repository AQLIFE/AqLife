using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;

namespace AqLife.Application.Business.Tag.Search
{
    public class TagSearch(QueryMapper queryMapper, IApplicationDbContext storage,IHttpContextAccessor httpContext,
     IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, TagEntity, TagDto, EntitySearchCriteria>(storage, httpContext, searchStrategies)
    {
        protected private override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected private override IQueryable<TagEntity> BuildBaseQuery(IQueryable<TagEntity> queryable, TagQuery query)
       => queryable;

        protected private override IQueryable<TagEntity> ApplyDefaultOrder(IQueryable<TagEntity> queryble, TagQuery query)
        => queryble.OrderBy(e => e.Name);
    }
}
