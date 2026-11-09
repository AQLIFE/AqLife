using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class UpdateTodoCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateTodoCommand, Guid>
{
    public async Task<Guid> Handle(UpdateTodoCommand q, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == q.UID, ct);
        t.UpdateDetails(q.EffectiveTitle, q.Description ?? t.Description, q.Priority);
        t.SetStatus(q.Status);
        return t.UID;
    }
}
