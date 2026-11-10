using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Entities;
using AqLife.Domain.Entities.Favorite;
using AqLife.Domain.Entities.File;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers;

[Mapper]
public partial class CategoryMapper : IViewMapper<CategoryEntity, CategoryDto>
{
    [MapperIgnoreSource(nameof(CategoryEntity.ParentId))]
    [MapProperty(nameof(CategoryEntity.Files.Count), nameof(CategoryDto.FileCount))]
    [MapProperty(nameof(CategoryEntity.Favorites.Count), nameof(CategoryDto.FavoriteCount))]
    [MapProperty(nameof(CategoryEntity.ParentCategory.CategoryName), nameof(CategoryDto.ParentName))]
    public partial CategoryDto ToDto(CategoryEntity source);
}

[Obsolete("设计取代:和File 业务相同的原因,转换功能交给底层Mapper,业务脱敏实现交由对应的 Search 类实现 ")]
public sealed class CategoryMapperServices(IHttpContextAccessor httpContextAccessor,IOptions<FilePolicyOption> filePolicy)
    //: IViewMapper<CategoryEntity, CategoryDto>
{
    /// <summary>
    /// 未认证请求只能看到公开业务统计。
    /// 查询 Handler 使用该策略选择 SQL 计数表达式，确保排序和返回值一致。
    /// </summary>
    public bool IsValid =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string[] AllowedBlogExtensions => filePolicy.Value.AllowedBlogExtensions;
    public string[] AllowedImageExtensions => filePolicy.Value.AllowedImageExtensions;
    public string[] AllowedVideoExtensions => filePolicy.Value.AllowedVideoExtensions;

    //public CategoryDto ToDto(CategoryEntity source)
    //{
    //    if( IsValid )
    //        return categoryMapper.ToDto(source);
    //    else
    //    {

    //    }

    //    return source

       
    //}

    public IQueryable<FileMetaEntity> BlogCount(CategoryEntity source)
    => source.Files.Where(file =>
            !file.IsTemplate &&
            AllowedBlogExtensions.Contains(file.Extension)).AsQueryable<FileMetaEntity>();

    public IQueryable<FileMetaEntity> TemplateCount(CategoryEntity source)
    => source.Files.Where(file =>
            file.IsTemplate &&
            AllowedBlogExtensions.Contains(file.Extension)).AsQueryable<FileMetaEntity>();

    public IQueryable<FileMetaEntity> ImageCount(CategoryEntity source)
    => source.Files.Where(file =>
            !file.IsTemplate &&
            AllowedImageExtensions.Contains(file.Extension)).AsQueryable<FileMetaEntity>();

    public IQueryable<FileMetaEntity> VideoCount(CategoryEntity source)
    => source.Files.Where(file =>
            !file.IsTemplate &&
            AllowedVideoExtensions.Contains(file.Extension)).AsQueryable<FileMetaEntity>();


    public IQueryable<FileMetaEntity> FileCount(CategoryEntity source, bool isPublished = true)
        => isPublished ?
        source.Files.Where(e => e.PublishMeta.PublishStatus != FileStatus.Published).AsQueryable<FileMetaEntity>() :
        source.Files.Where(e => e.PublishMeta.PublishStatus == FileStatus.Published).AsQueryable<FileMetaEntity>();
    public IQueryable<IQueryable<FileMetaEntity>> FileCount(IQueryable<CategoryEntity> source, bool isPublished = true)
        =>  source.Select(e => isPublished ? 
            e.Files.Where(file=>file.PublishMeta.PublishStatus != FileStatus.Published).AsQueryable<FileMetaEntity>() :
            e.Files.Where(file=>file.PublishMeta.PublishStatus == FileStatus.Published).AsQueryable<FileMetaEntity>()
        );

    public IQueryable<FavoriteItemEntity> FavoriteCount(CategoryEntity source,bool isPublished=true)
    => source.Favorites.Where(e => e.IsPublished).AsQueryable<FavoriteItemEntity>();

}

