using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.Tag
{
    [Mapper]
    public partial class TagMapper : IViewMapper<TagEntity, TagDto>, ICreateMapper<TagEntity, CreateTagCommand>,
        ISearchMapper<TagQuery, PageResult<TagDto>, EntitySearchCriteria>
    {
        [MapperIgnoreSource(nameof(TagQuery.Page))]
        [MapperIgnoreSource(nameof(TagQuery.PageSize))]
        [MapProperty(nameof(TagQuery.Tag), nameof(EntitySearchCriteria.Keyword))]
        public partial EntitySearchCriteria ToCriteria(TagQuery query);

        [MapperIgnoreSource(nameof(TagEntity.FileTags))]
        public partial TagDto ToDto(TagEntity entity);

        [MapperIgnoreTarget(nameof(TagEntity.UID))]
        [MapperIgnoreTarget(nameof(TagEntity.FileTags))]
        public partial TagEntity ToEntity(CreateTagCommand command);
    }
}
