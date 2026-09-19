using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Business.Tag;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.File
{
    [Mapper]
    public partial class FileMapper(TagMapper tagMapper)
        : IViewMapper<FileMetaEntity, FileDto>
    {
        //[MapperIgnoreSource(nameof(FileMetaEntity.PublishAt))]
        //[MapperIgnoreSource(nameof(FileMetaEntity.PublishStatus)]
        [MapperIgnoreSource(nameof(FileMetaEntity.StorageKey))]
        [MapProperty(nameof(FileMetaEntity.Extension), nameof(FileDto.FileType))]
        [MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileDto.Tags))]
        public partial FileDto ToDto(FileMetaEntity source);

        public string Convert(DateTimeOffset dateTime) => dateTime.ToString("yyyy-MM-dd");
        
        public TagDto Convert(FileTagEntity tagEntity) => tagMapper.ToDto(tagEntity.Tag);

    }

    public class FileViewMapper(IHttpContextAccessor httpContext, FileMapper fileMapper)
    {
        public FileDto ToDto(FileMetaEntity source)
            => httpContext.HttpContext?.User.Identity?.IsAuthenticated == true 
            ? fileMapper.ToDto(source) 
            : source.Desensitize( 
                source.FileTags.Select(e=> fileMapper.Convert(e)),
                fileMapper.Convert(source.UploadTime)
                );

    }

    internal static class SafeDesensitization
    {
        internal static FileDto Desensitize(this FileMetaEntity source,IEnumerable<TagDto> tagsDto,string dateTime)
        => new (
            UID:source.UID,
            FileName:source.FileName,
            Tags:tagsDto,
            PublishAt:null,
            PublishStatus:null,
            FileSize:source.FileSize,
            FileHash:source.FileHash,
            UploadTime:dateTime,
            FileType:source.Extension,
            FileIntroduction: source.FileIntroduction
        );
    }

}
