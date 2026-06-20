using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Service.MapperService
{
    [Mapper]
    public partial class TagMapper: IGenMapper<TagEntity, TagDto>
    {
        [MapperIgnoreSource(nameof(TagEntity.UID))]
        [MapperIgnoreSource(nameof(TagEntity.FileTags))]
        public partial TagDto ToDto(TagEntity entity);

        [MapperIgnoreTarget(nameof(TagEntity.UID))]
        [MapperIgnoreTarget(nameof(TagEntity.FileTags))]
        public partial TagEntity ToEntity(TagDto dto);

        [MapperIgnoreTarget(nameof(TagEntity.UID))]
        [MapperIgnoreTarget(nameof(TagEntity.FileTags))]
        public partial void UpdateEntity(TagDto dto, TagEntity entity);
    }
}
