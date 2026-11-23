using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Todo;
using AqLife.Domain.Entities.DevPlan;
using MediatR;

namespace AqLife.Application.Plan.Todo.Handler;

public sealed class CreateTodoCommandHandler(
    IPlanPersistence persistence,
    ICreateMapper<TodoEntity, CreateTodoCommand> mapper) : IRequestHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand q, CancellationToken ct)
    {
        var todo = mapper.ToEntity(q);
        todo.LayoutEntity = new(todoId: todo.UID, todo.PlanId, 10, 10);
        await persistence.AddTodoAsync(todo, ct);
        return todo.UID;
    }
}