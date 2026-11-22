using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Core.Search;
using Riok.Mapperly.Abstractions;

namespace Category;

[Mapper]
public partial class CategoryMapper : IViewMapper<CategoryEntity, CategoryDto>, ICreateMapper<CategoryEntity, CreateCategoryCommand>,
    ISearchMapper<CategoryQuery, PageResult<CategoryDto>, EntitySearchCriteria>
{
    [MapperIgnoreSource(nameof(CategoryQuery.Page))]
    [MapperIgnoreSource(nameof(CategoryQuery.PageSize))]
    [MapperIgnoreSource(nameof(CategoryQuery.SearchType))]
    [MapperIgnoreTarget(nameof(EntitySearchCriteria.Keyword))]
    public partial EntitySearchCriteria ToCriteria(CategoryQuery query);

    [MapperIgnoreSource(nameof(CategoryEntity.ParentCategory))]
    [MapperIgnoreSource(nameof(CategoryEntity.Files))]
    [MapperIgnoreSource(nameof(CategoryEntity.Favorites))]
    [MapProperty(nameof(CategoryEntity), nameof(CategoryDto.ResultIds), Use = nameof(ToResultGuid))]
    [MapProperty(nameof(CategoryEntity.UID), nameof(CategoryDto.Uid))]
    [MapProperty(nameof(CategoryEntity.CategoryDescription), nameof(CategoryDto.CategoryDescription))]
    [MapProperty(nameof(CategoryEntity.CategoryName), nameof(CategoryDto.CategoryName))]
    [MapProperty(nameof(CategoryEntity.CategoryCount), nameof(CategoryDto.Count))]
    public partial CategoryDto ToDto(CategoryEntity source);

    public static IEnumerable<Guid> ToResultGuid(CategoryEntity source)
    {
        if (source.Files.Count > 0) return source.Files.Select(entity => entity.UID);
        if (source.Favorites.Count > 0) return source.Favorites.Select(entity => entity.UID);
        return [];
    }

    [MapProperty(nameof(CreateCategoryCommand.Desc), nameof(CategoryEntity.CategoryDescription))]
    [MapProperty(nameof(CreateCategoryCommand.Name), nameof(CategoryEntity.CategoryName))]
    [MapperIgnoreTarget(nameof(CategoryEntity.UID))]
    [MapperIgnoreTarget(nameof(CategoryEntity.Files))]
    [MapperIgnoreTarget(nameof(CategoryEntity.Favorites))]
    [MapperIgnoreTarget(nameof(CategoryEntity.CategoryCount))]
    [MapperIgnoreTarget(nameof(CategoryEntity.ParentCategory))]
    public partial CategoryEntity ToEntity(CreateCategoryCommand source);
}