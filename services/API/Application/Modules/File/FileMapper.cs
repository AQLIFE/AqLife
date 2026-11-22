using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using Core.Search;
using Riok.Mapperly.Abstractions;

namespace File;

[Mapper]
public partial class FileMapper :
    IViewMapper<FileMetaEntity, FileDto>,
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

    public IEnumerable<TagDto> Convert(ICollection<FileTagEntity> source)
        => source.Select(fileTag => new TagDto(
            fileTag.Tag.UID.ToString(),
            fileTag.Tag.Name,
            fileTag.Tag.AliasName));

    public CategoryDto Convert(CategoryEntity source)
        => new(
            source.UID,
            source.CategoryName,
            source.CategoryDescription,
            source.CategoryCount,
            source.ParentId,
            []);

    public FileMetaEntity ToEntity(CreateFileCommand command)
        => new(command.File, command.CategoryId, isTemplate: command.IsTemplate);

    public IEnumerable<FileMetaEntity> ToEntity(UploadFilesCommand command)
        => command.Files.Select(file => new FileMetaEntity(file, command.categoryId));
}