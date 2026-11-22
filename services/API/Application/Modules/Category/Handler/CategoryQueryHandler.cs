using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities;
using Core.Mappers;
using MediatR;

namespace Category.Handler;

public sealed class CategoryQueryHandler(
    ISearch<CategoryQuery, PageResult<CategoryDto>, CategoryEntity> categorySearch,
    PageResultMapper<CategoryEntity, CategoryDto> pageResultMapper)
    : IRequestHandler<CategoryQuery, PageResult<CategoryDto>?>
{
    public async Task<PageResult<CategoryDto>?> Handle(CategoryQuery query, CancellationToken ct)
    {
        var categories = await categorySearch.SearchPageAsync(query, ct);
        return pageResultMapper.ToDto(categories);
    }
}