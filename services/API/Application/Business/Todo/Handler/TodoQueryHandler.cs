using MediatR;
using MyLife.Application.Business.Todo.Search;
using MyLife.Domain.Command;
using MyLife.Shared.IView;

namespace MyLife.Application.Business.Todo.Handler
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
