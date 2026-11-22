using AqLife.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Plan.Todo.Handler;

public sealed class DeleteTodoCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteTodoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTodoCommand q, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == q.UID, ct);
        db.Todo.Remove(t);
        return Unit.Value;
    }
}
