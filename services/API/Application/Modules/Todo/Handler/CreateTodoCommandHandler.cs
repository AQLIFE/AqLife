using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.DevPlan;
using MediatR;

namespace AqLife.Application.Modules.Todo.Handler;

public sealed class CreateTodoCommandHandler(
    IApplicationDbContext db,
    ICreateMapper<TodoEntity, CreateTodoCommand> mapper) : IRequestHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand q, CancellationToken ct)
    {
        var todo = mapper.ToEntity(q);
        todo.LayoutEntity = new(todoId: todo.UID, todo.PlanId, 10, 10);
        await db.Todo.AddAsync(todo, ct);
        return todo.UID;
    }
}