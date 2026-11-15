using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class ArchiveTodoCommandHandler(IApplicationDbContext db) : IRequestHandler<ArchiveTodoCommand, Guid>
{
    public async Task<Guid> Handle(ArchiveTodoCommand q, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == q.UID, ct);
        t.Archive();
        return t.UID;
    }
}
