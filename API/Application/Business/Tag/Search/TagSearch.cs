using MyLife.Application.Abstractions.Search;
using MyLife.Application.Mapper;
using MyLife.Application.Search;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Infrastructure.Persistence;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Tag.Search
{
    public class TagSearch(QueryMapper queryMapper, AppStorage storage
    , IEnumerable<ISearchStrategy<TagEntity, EntitySearchCriteria>> searchStrategies
    ) : BaseSearch<TagQuery, TagEntity, TagDto, EntitySearchCriteria>(storage, searchStrategies)
    {
        protected override EntitySearchCriteria MapToCriteria(TagQuery query)
        => queryMapper.ToCriteria(query);

        protected override async Task<IQueryable<TagEntity>> BuildBaseQueryAsync(IQueryable<TagEntity> queryable, TagQuery query)
       => queryable;
    }
}
