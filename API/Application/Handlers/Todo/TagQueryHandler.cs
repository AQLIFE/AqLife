using MediatR;
using MyLife.Domain.Command;
using MyLife.Service.Mappings;
using MyLife.Service.Search.Todo;
using MyLife.Shared.IView;

namespace MyLife.Application.Handlers.Todo
{
    public class TagQueryHandler(TodoSearch search,TodoMapper mapper): IRequestHandler<TodoQuery, IEnumerable<TodoDto>?>
    {
        public async Task<IEnumerable<TodoDto>?> Handle(TodoQuery request, CancellationToken cancellationToken)
        {
            var result = await search.SearchAsync(request, cancellationToken);
            return result?.Select(e => mapper.ToDto(e)) ?? [];
        }
    }
}
