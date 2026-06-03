using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper : IGenMapper<FileMetaEntity, FileDto>
    {
        //[MapperIgnoreSource(nameof(FileMetaEntity.UID))]
        [MapperIgnoreSource(nameof(FileMetaEntity.DesensitizationName))]
        [MapperIgnoreSource(nameof(FileMetaEntity.Extension))]
        public partial FileDto ToDto(FileMetaEntity obj);

        //[MapperIgnoreTarget(nameof(FileMetaEntity.UID))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.DesensitizationName))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        public partial FileMetaEntity ToEntity(FileDto dto);

        //[MapperIgnoreTarget(nameof(FileMetaEntity.UID))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.DesensitizationName))]
        public partial void UpdateEntity(FileDto dto,FileMetaEntity entity);
        private string Convert(DateTime dateTime)=> dateTime.ToString("yyyy-MM-dd");
    }


}
