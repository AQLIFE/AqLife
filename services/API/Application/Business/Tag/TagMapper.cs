using MyLife.Application.Abstractions.Mapper;
using MyLife.Domain.Command;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Business.Tag
{
    [Mapper]
    public partial class TagMapper : IViewMapper<TagEntity, TagDto>, ICreateMapper<TagEntity, CreateTagCommand>
    {
        [MapperIgnoreSource(nameof(TagEntity.FileTags))]
        public partial TagDto ToDto(TagEntity entity);

        [MapperIgnoreTarget(nameof(TagEntity.UID))]
        [MapperIgnoreTarget(nameof(TagEntity.FileTags))]
        public partial TagEntity ToEntity(CreateTagCommand command);
    }
}
