using AqLife.Application.Abstractions.Mapper;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using Riok.Mapperly.Abstractions;

namespace AqLife.Application.Mappers;

[Mapper]
public partial class CategoryMapper : IViewMapper<CategoryEntity, CategoryDto>
{
    [MapperIgnoreSource(nameof(CategoryEntity.ParentId))]
    [MapProperty(nameof(CategoryEntity.Files.Count), nameof(CategoryDto.CategoryCount))]
    [MapProperty(nameof(CategoryEntity.ParentCategory.CategoryName), nameof(CategoryDto.ParentName))]
    public partial CategoryDto ToDto(CategoryEntity source);
}
