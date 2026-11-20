using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Modules.Todo.Handler;

public sealed class AddTodoDependencyCommandHandler(
    IApplicationDbContext db,
    ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand> mapper) : IRequestHandler<AddTodoDependencyCommand, Guid>
{
    public async Task<Guid> Handle(AddTodoDependencyCommand q, CancellationToken ct)
    {
        var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == q.PlanId, ct);
        var dependency = mapper.ToEntity(q);
        await db.TodoDependencies.AddAsync(dependency, ct);
        //plan.IncrementDependencyVersion(); // 自动实现
        return dependency.UID;
    }
}

public sealed class RemoveTodoDependencyCommandHandler(IApplicationDbContext db) : IRequestHandler<RemoveTodoDependencyCommand, Unit>
{
    public async Task<Unit> Handle(RemoveTodoDependencyCommand q, CancellationToken ct)
    {
        TodoDependencyEntity dependencyEntity = await db.TodoDependencies.SingleAsync(e => e.UID == q.UID, ct);// 默认没有重复UID 且 于属于 planId 所指的Plan; RemoveTodoDependencyLegalityValidator 业务会保障这点
        db.TodoDependencies.Remove(dependencyEntity);
        return Unit.Value;
    }
}
