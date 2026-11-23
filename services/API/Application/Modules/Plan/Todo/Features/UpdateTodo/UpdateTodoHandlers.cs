using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Todo;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.Todo.Handler;

public sealed class UpdateTodoDetailsCommandHandler(IPlanPersistence persistence) : IRequestHandler<UpdateTodoCommand, Guid>
{
    public async Task<Guid> Handle(UpdateTodoCommand command, CancellationToken ct)
    {
        var t = await persistence.Todos.FirstAsync(x => x.UID == command.UID, ct);
        t.UpdateDetails(command.Title, command.Description ?? t.Description, command.SortOrder);
        return t.UID;
    }
}

public sealed class UpdateTodoStatusCommandHandler(IPlanPersistence persistence) : IRequestHandler<UpdateTodoStatusCommand, Guid>
{
    public async Task<Guid> Handle(UpdateTodoStatusCommand command, CancellationToken ct)
    {
        var t = await persistence.Todos.FirstAsync(x => x.UID == command.UID, ct);
        t.SetStatus(command.Status);
        return t.UID;
    }
}

public sealed class RestoreTodoCommandHandler(IPlanPersistence persistence) : IRequestHandler<RestoreTodoCommand, Guid>
{
    public async Task<Guid> Handle(RestoreTodoCommand q, CancellationToken ct)
    {
        var t = await persistence.Todos.FirstAsync(x => x.UID == q.UID, ct);
        t.Restore();
        return t.UID;
    }
}

public sealed class ArchiveTodoCommandHandler(IPlanPersistence persistence) : IRequestHandler<ArchiveTodoCommand, Guid>
{
    public async Task<Guid> Handle(ArchiveTodoCommand q, CancellationToken ct)
    {
        var t = await persistence.Todos.FirstAsync(x => x.UID == q.UID, ct);
        t.Archive();
        return t.UID;
    }
}
