using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Business.Tag;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.File
{
    [Mapper]
    public partial class FileMapper(IViewMapper<TagEntity,TagDto> tagMapper)
        : IViewMapper<FileMetaEntity, FileDto>,
        ICreateMapper<FileMetaEntity, IFormFile>
    {
        [MapperIgnoreSource(nameof(FileMetaEntity.StorageKey))]
        [MapProperty(nameof(FileMetaEntity.Extension), nameof(FileDto.FileType))]
        [MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileDto.Tags))]
        [MapProperty(nameof(FileMetaEntity.PublishMeta.PublishStatus), nameof(FileDto.PublishStatus))]
        [MapProperty(nameof(FileMetaEntity.PublishMeta.PublishAt), nameof(FileDto.PublishAt))]
        [MapProperty(nameof(FileMetaEntity.InteractionMeta.ViewCount), nameof(FileDto.ViewCount))]
        [MapProperty(nameof(FileMetaEntity.FileIntroduction), nameof(FileDto.FileIntroduction))]
        public partial FileDto ToDto(FileMetaEntity source);

        public string Convert(DateTimeOffset dateTime) => dateTime.ToString("yyyy-MM-dd");
        
        public TagDto Convert(FileTagEntity tagEntity) => tagMapper.ToDto(tagEntity.Tag);

        /// <summary>
        /// Creates a new FileMetaEntity from an IFormFile.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public partial FileMetaEntity ToEntity(IFormFile file);
        

    }

    
    public partial class FileMappingService(IHttpContextAccessor httpContext, FileMapper fileMapper,UploadContext uploadContext)
        : IViewMapper<FileMetaEntity, FileDto>,
        ICreateMapper<FileMetaEntity, IFormFile>
    {
        public FileDto ToDto(FileMetaEntity source)
            => httpContext.HttpContext?.User.Identity?.IsAuthenticated == true 
            ? fileMapper.ToDto(source) 
            : source.Desensitize( 
                source.FileTags.Select(fileMapper.Convert),
                fileMapper.Convert(source.UploadTime)
                );

        public FileMetaEntity ToEntity(IFormFile file)
        {
            return fileMapper.ToEntity(file).UpdateHash(uploadContext.FileHashes[file]);
        }
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
            FileIntroduction: source.FileIntroduction,
            ViewCount:source.InteractionMeta.ViewCount
        );
    }

}
