using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using MediatR;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class CreateTodoCommandHandler(
    IApplicationDbContext db,
    ICreateMapper<TodoEntity, CreateTodoCommand> mapper) : IRequestHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand q, CancellationToken ct)
    {
        var todo = mapper.ToEntity(q);
        todo.LayoutEntity = new(todoId:todo.UID,todo.PlanId,0,0);
        await db.Todo.AddAsync(todo, ct);
        return todo.UID;
    }
}