using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper : IGenMapper<FileMetaEntity, FileMetadataDto>
    {
        //[MapperIgnoreSource(nameof(FileMetaEntity.UID))]
        [MapperIgnoreSource(nameof(FileMetaEntity.Extension))]
        public partial FileMetadataDto ToDto(FileMetaEntity obj);

        //[MapperIgnoreTarget(nameof(FileMetaEntity.UID))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        public partial FileMetaEntity ToEntity(FileMetadataDto dto);

        //[MapperIgnoreTarget(nameof(FileMetaEntity.UID))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        public partial void UpdateEntity(FileMetadataDto dto,FileMetaEntity entity);
        private string Convert(DateTime dateTime)=> dateTime.ToString("yyyy-MM-dd");
    }


}
