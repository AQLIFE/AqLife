using AqLife.Application.Business.Todo.Search;
using AqLife.Domain.Command;
using AqLife.Shared.IView;
using MediatR;
using AqLife.Application.Business.Todo;

namespace AqLife.Application.Business.Todo.Handler
{
    public class TodoQueryHandler(TodoSearch search, TodoMapper mapper) : IRequestHandler<TodoQuery, IEnumerable<TodoDto>?>
    {
        public async Task<IEnumerable<TodoDto>?> Handle(TodoQuery request, CancellationToken cancellationToken)
        {
            var result = await search.SearchAsync(request, cancellationToken);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
