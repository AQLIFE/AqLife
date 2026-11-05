using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Todo;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class CreateTodoCommandHandler(
    IApplicationDbContext db,
    TodoMapper mapper) : IRequestHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand q, CancellationToken ct)
    {
        var todo = mapper.ToEntity(q);
        await db.Todo.AddAsync(todo, ct);
        return todo.UID;
    }
}