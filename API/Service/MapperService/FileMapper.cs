using MyLife.Data.Entities;
using MyLife.Service.MapperService;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper(TagMapper tagMapper) : IGenMapper<FileMetaEntity, FileMetadataDto>
    {
        [MapProperty(nameof(FileMetaEntity.Extension), nameof(FileMetadataDto.FileType))]
        [MapProperty(nameof(FileMetaEntity.FileTags),nameof(FileMetadataDto.Tags))]
        public partial FileMetadataDto ToDto(FileMetaEntity obj);

        [MapperIgnoreTarget(nameof(FileMetaEntity.FileTags))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        [MapperIgnoreSource(nameof(FileMetadataDto.Tags))]
        [MapperIgnoreSource(nameof(FileMetadataDto.FileType))]
        public partial FileMetaEntity ToEntity(FileMetadataDto dto);

        //[MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileMetadataDto.Tags))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.FileTags))]
        [MapperIgnoreTarget(nameof(FileMetaEntity.Extension))]
        [MapperIgnoreSource(nameof(FileMetadataDto.Tags))]
        [MapperIgnoreSource(nameof(FileMetadataDto.FileType))]
        public partial void UpdateEntity(FileMetadataDto dto, FileMetaEntity entity);
        private string Convert(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");
        // Mapperly 会自动循环处理 FileTags 集合中的每一个项 [cite: 198]
        private TagDto Convert(FileTagEntity tagEntity)
        {
            return tagMapper.ToDto(tagEntity.Tag);
        }

    }


}
