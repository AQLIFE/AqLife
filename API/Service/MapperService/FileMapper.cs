using MyLife.Data.Entities;
using MyLife.Service.ServiceInterfaces;
using MyLife.Shared.DTOs;
using Riok.Mapperly.Abstractions;

namespace MyLife.Service.Mappings
{
    [Mapper]
    public partial class FileMapper : IGenMapper<FileMetaEntity, FileMetadataDto>
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
        private string Convert(FileTagEntity tagEntity)
        {
            // 💡 只要 Service 层做了 Include，这里就不会是 null
            // 这里直接跨表取值：tagEntity -> 导航属性 Tag -> TagName
            return tagEntity.Tag?.Name ?? "未命名标签";
        }

    }


}
