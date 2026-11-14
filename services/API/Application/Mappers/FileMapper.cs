using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Mappers;
using AqLife.Application.Search;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using Microsoft.AspNetCore.Http;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Business.File
{
    [Mapper]
    public partial class FileMapper(IViewMapper<TagEntity, TagDto> tagMapper,IViewMapper<CategoryEntity, CategoryDto> categoryMapper)
        : IViewMapper<FileMetaEntity, FileDto>,
        ICreateMapper<FileMetaEntity, CreateFileCommand>,
        IBaseListEntityMapper<FileMetaEntity, UploadFilesCommand>,
        ISearchMapper<FileQuery, PageResult<FileDto>, FileSearchCriteria>

    {
        [MapperIgnoreSource(nameof(FileQuery.Page))]
        [MapperIgnoreSource(nameof(FileQuery.PageSize))]
        [MapperIgnoreSource(nameof(FileQuery.Order))]
        [MapperIgnoreSource(nameof(FileQuery.Scope))]
        [MapProperty(nameof(FileQuery.Title), nameof(FileSearchCriteria.Keyword))]
        public partial FileSearchCriteria ToCriteria(FileQuery query);

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

        //[MapperIgnoreSource(nameof(FileMetaEntity.StorageKey))]
        //[MapperIgnoreSource(nameof(FileMetaEntity.CategoryUID))]
        //[MapProperty(nameof(FileMetaEntity.Extension), nameof(FileDto.FileType))]
        //[MapProperty(nameof(FileMetaEntity.FileTags), nameof(FileDto.Tags))]
        //[MapProperty(nameof(FileMetaEntity.PublishMeta.PublishStatus), nameof(FileDto.PublishStatus))]
        //[MapProperty(nameof(FileMetaEntity.PublishMeta.PublishAt), nameof(FileDto.PublishAt), Use = nameof(Cleaning))]
        //[MapProperty(nameof(FileMetaEntity.InteractionMeta.ViewCount), nameof(FileDto.ViewCount))]
        //[MapProperty(nameof(FileMetaEntity.FileIntroduction), nameof(FileDto.FileIntroduction))]
        //[MapProperty(nameof(FileMetaEntity.Category), nameof(FileDto.Category))]
        //public partial FileDto ToVisitorDto(FileMetaEntity source);
        //public static DateTimeOffset? Cleaning(DateTimeOffset? original)
        //    => original is not DateTimeOffset source ? null : new(source.Date, source.Offset);

        public TagDto Convert(FileTagEntity tagEntity) => tagMapper.ToDto(tagEntity.Tag);
        public CategoryDto Convert(CategoryEntity categoryEntity) => categoryMapper.ToDto(categoryEntity) ;
        //public static DateTimeOffset? Acquiescence(this DateTimeOffset? source) => source;


        /// <summary>
        /// Creates a new FileMetaEntity from an IFormFile.
        /// </summary>
        public FileMetaEntity ToEntity(CreateFileCommand command)
            => new(command.File, command.categoryId, isTemplate: command.IsTemplate);

        //[MapperIgnoreTarget(nameof(FileMetaEntity.UID))]
        //[MapperIgnoreTarget(nameof(FileMetaEntity.InteractionMeta))]
        //[MapperIgnoreTarget(nameof(FileMetaEntity.Category))]
        //[MapperIgnoreTarget(nameof(FileMetaEntity.FileTags))]
        //[MapperIgnoreTarget(nameof(FileMetaEntity.PublishMeta))]
        //[MapperIgnoreSource(nameof(CreateFileCommand.categoryId))]
        public IEnumerable<FileMetaEntity> ToEntity(UploadFilesCommand command)
            => command.Files.Select(e => new FileMetaEntity(e, command.categoryId));
    }
}