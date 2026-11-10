using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Category.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Category.Handler;

public sealed class CategoryQueryHandler(CategorySearch categorySearch,PageResultMapper<CategoryEntity,CategoryDto> pageResultMapper)
    : IRequestHandler<CategoryQuery, PageResult<CategoryDto>?>
{
    public async Task<PageResult<CategoryDto>?> Handle(CategoryQuery query, CancellationToken ct)
    {
        var categories = await categorySearch.SearchPageAsync(query, ct);
        return pageResultMapper.ToDto(categories);
    }
}
