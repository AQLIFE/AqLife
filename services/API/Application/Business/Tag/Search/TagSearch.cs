using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mappers;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using MyLife.Application.Abstractions.Persistence;

namespace MyLife.Application.Business.Tag.Search
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
