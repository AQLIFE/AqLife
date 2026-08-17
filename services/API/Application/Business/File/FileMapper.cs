using MyLife.Application.Abstractions.Mapper;
using MyLife.Application.Business.Tag;
using MyLife.Domain.Entities;
using MyLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace MyLife.Application.Business.File
{
    [Mapper]
    public partial class FileMapper(TagMapper tagMapper)
        : IViewMapper<FileMetaEntity, FileDto>
    {
        [MapperIgnoreSource(nameof(FileMetaEntity.StorageName))]
        [MapProperty(nameof(FileMetaEntity.Extension), nameof(FileDto.FileType))]
        [MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileDto.Tags))]
        public partial FileDto ToDto(FileMetaEntity source);
        private string Convert(DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");
        // Mapperly 会自动循环处理 FileTags 集合中的每一个项 [cite: 198]
        private TagDto Convert(FileTagEntity tagEntity) => tagMapper.ToDto(tagEntity.Tag);

    }


}
