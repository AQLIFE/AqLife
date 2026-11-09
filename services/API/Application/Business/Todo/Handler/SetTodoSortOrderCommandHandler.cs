using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class SetTodoSortOrderCommandHandler(IApplicationDbContext db) : IRequestHandler<SetTodoSortOrderCommand, Guid>
{
    public async Task<Guid> Handle(SetTodoSortOrderCommand q, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == q.UID, ct);
        t.SetSortOrder(q.SortOrder);
        return t.UID;
    }
}
