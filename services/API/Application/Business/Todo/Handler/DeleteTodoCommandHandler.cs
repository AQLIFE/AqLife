using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class DeleteTodoCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteTodoCommand, Unit>
{
    public async Task<Unit> Handle(DeleteTodoCommand q, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == q.UID, ct);
        db.Todo.Remove(t);
        return Unit.Value;
    }
}
