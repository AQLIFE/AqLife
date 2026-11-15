using AqLife.Application.Abstractions.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.IView;
using MediatR;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class TodoQueryHandler(
    ISearch<TodoQuery, TodoEntity> todoSearch,
    PageResultMapper<TodoEntity, TodoDto> pageMapper) : IRequestHandler<TodoQuery, PageResult<TodoDto>>
{
    public async Task<PageResult<TodoDto>> Handle(TodoQuery q, CancellationToken ct)
    {
        PageResult<TodoEntity> result = await todoSearch.SearchPageAsync(q, ct);
        return pageMapper.ToDto(result);
    }
}