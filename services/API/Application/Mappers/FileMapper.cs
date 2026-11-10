using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Mappers;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Tools;
using Microsoft.AspNetCore.Http;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.File
{
    [Mapper]
    public partial class FileMapper(
        IViewMapper<TagEntity, TagDto> tagMapper,
        IViewMapper<CategoryEntity, CategoryDto> categoryMapper)
        : IViewMapper<FileMetaEntity, FileDto>,
        ICreateMapper<FileMetaEntity, IFormFile>
    {
        [MapperIgnoreSource(nameof(FileMetaEntity.StorageKey))]
        [MapperIgnoreSource(nameof(FileMetaEntity.CategoryUID))]
        [MapProperty(nameof(FileMetaEntity.Extension), nameof(FileDto.FileType))]
        [MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileDto.Tags))]
        [MapProperty(nameof(FileMetaEntity.PublishMeta.PublishStatus), nameof(FileDto.PublishStatus))]
        [MapProperty(nameof(FileMetaEntity.PublishMeta.PublishAt), nameof(FileDto.PublishAt))]
        [MapProperty(nameof(FileMetaEntity.InteractionMeta.ViewCount), nameof(FileDto.ViewCount))]
        [MapProperty(nameof(FileMetaEntity.FileIntroduction), nameof(FileDto.FileIntroduction))]
        [MapProperty(nameof(FileMetaEntity.Category), nameof(FileDto.Category))]
        public partial FileDto ToDto(FileMetaEntity source);

        public string Convert(DateTimeOffset dateTime) => dateTime.ToString("yyyy-MM-dd");

        public TagDto Convert(FileTagEntity tagEntity) => tagMapper.ToDto(tagEntity.Tag);
        public CategoryDto? Convert(CategoryEntity? categoryEntity) => categoryEntity is not null ? categoryMapper.ToDto(categoryEntity) : null;

        /// <summary>
        /// Creates a new FileMetaEntity from an IFormFile.
        /// </summary>
        public partial FileMetaEntity ToEntity(IFormFile file);
    }

    /// <summary>
    /// 封装后的 FileMapper，用于完成基于身份和会话上下文的数据转换。
    /// </summary>
    [Obsolete("设计取代:现在使用对应业务Search类进行全查询时会自动完成脱敏")]
    public partial class FileMappingService(
        IHttpContextAccessor httpContext,
        FileMapper fileMapper,
        UploadContext uploadContext)
        : IViewMapper<FileMetaEntity, FileDto>,
        ICreateMapper<FileMetaEntity, IFormFile>
    {
        public FileDto ToDto(FileMetaEntity source)
            => httpContext.HttpContext?.User.Identity?.IsAuthenticated == true
            ? fileMapper.ToDto(source)
            : source.Desensitize(
                source.FileTags.Select(fileMapper.Convert),
                fileMapper.Convert(source.UploadTime),
                fileMapper.Convert(source.Category));

        public FileMetaEntity ToEntity(IFormFile file)
            => fileMapper.ToEntity(file).UpdateHash(uploadContext.FileHashes[file]);
    }
    [Obsolete("设计取代:现在在 FileMetaEntity 中已原生实现预期的脱敏效果")]
    internal static class SafeDesensitization
    {
        internal static FileDto Desensitize(
            this FileMetaEntity source,
            IEnumerable<TagDto> tagsDto,
            string dateTime,
            CategoryDto? categoryDto)
            => new(
                UID: source.UID,
                FileName: source.FileName,
                Tags: tagsDto,
                PublishAt: null,
                PublishStatus: null,
                FileSize: source.FileSize,
                FileHash: source.FileHash,
                UploadTime: dateTime,
                FileType: source.Extension,
                FileIntroduction: source.FileIntroduction,
                ViewCount: source.InteractionMeta.ViewCount,
                Version: source.Version,
                IsTemplate: source.IsTemplate,
                Category: categoryDto);
    }
}
