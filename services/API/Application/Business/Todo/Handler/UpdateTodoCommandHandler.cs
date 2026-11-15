using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class UpdateTodoDetailsCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateTodoCommand, Guid>
{
    public async Task<Guid> Handle(UpdateTodoCommand command, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == command.UID, ct);
        t.UpdateDetails(command.Title, command.Description ?? t.Description, command.SortOrder);
        return t.UID;
    }
}

public sealed class UpdateTodoStatusCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateTodoStatusCommand, Guid>
{
    public async Task<Guid> Handle(UpdateTodoStatusCommand command, CancellationToken ct)
    {
        var t = await db.Todo.FirstAsync(x => x.UID == command.UID, ct);
        t.SetStatus(command.Status);
        return t.UID;
    }
}
