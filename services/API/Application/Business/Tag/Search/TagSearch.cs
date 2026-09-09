using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Abstractions.Search;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using AqLife.Application.Mappers;

namespace AqLife.Application.Business.Tag.Search
{
    public class TagSearch(QueryMapper queryMapper, IApplicationDbContext storage
    , IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, TagEntity, TagDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TagEntity>> BuildBaseQueryAsync(IQueryable<TagEntity> queryable, TagQuery query)
       => queryable;
    }
}
