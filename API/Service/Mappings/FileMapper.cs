using MyLife.Data.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper : IGenericsMapper<FileIndexEntity, FileDto>
    {
        [MapperIgnoreSource(nameof(FileIndexEntity.Uuid))]
        [MapperIgnoreSource(nameof(FileIndexEntity.DesensitizationName))]
        public partial FileDto Desensitization(FileIndexEntity obj);


        [MapperIgnoreTarget(nameof(FileIndexEntity.Uuid))]
        [MapperIgnoreTarget(nameof(FileIndexEntity.DesensitizationName))]
        public partial FileIndexEntity Assembly(FileDto dto);
    }
}
