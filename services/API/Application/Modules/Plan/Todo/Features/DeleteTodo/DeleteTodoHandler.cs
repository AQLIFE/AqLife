using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Todo;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.Todo.Handler;

public sealed class DeleteTodoCommandHandler(IPlanPersistence persistence) : IRequestHandler<DeleteTodoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTodoCommand q, CancellationToken ct)
    {
        var t = await persistence.Todos.FirstAsync(x => x.UID == q.UID, ct);
        persistence.RemoveTodo(t);
        return Unit.Value;
    }
}
