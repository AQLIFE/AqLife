using AqLife.Application.Business.Todo;
using AqLife.Application.Business.Todo.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Shared.IView;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AqLife.Application.Business.Todo.Handler
{
    public class TodoQueryHandler(TodoSearch search, PageResultMapper<TodoEntity,TodoDto> mapper) : IRequestHandler<TodoQuery, PageResult<TodoDto>>
    {
        public async Task<PageResult<TodoDto>> Handle(TodoQuery query, CancellationToken cancellationToken)
        {
            var result = await search.SearchPageAsync(query, cancellationToken);
            return mapper.ToDto(result);
        }
    }
}
