using MyLife.Data.Entities;
using MyLife.Service.Interfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper : IGenericsMapper<FileMetaEntity, FileDto>
    {
        [MapperIgnoreSource(nameof(FileMetaEntity.Uuid))]
        [MapperIgnoreSource(nameof(FileMetaEntity.DesensitizationName))]
        public partial FileDto Desensitization(FileMetaEntity obj);

        //public partial List<FileDto> Desensitization(List<FileMetaEntity> objList);

        [MapperIgnoreTarget(nameof(FileMetaEntity.Uuid))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.DesensitizationName))]
        public partial FileMetaEntity Assembly(FileDto dto);

        //public partial List<FileMetaEntity> Assembly(List<FileDto> dtoList);
        private string MapDateTime(DateTime dateTime)
        => dateTime.ToString("yyyy-MM-dd");
    }


}
