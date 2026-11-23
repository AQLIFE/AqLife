using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Todo;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.Todo.Handler;

public sealed class AddTodoDependencyCommandHandler(
    IPlanPersistence persistence,
    ICreateMapper<TodoDependencyEntity, AddTodoDependencyCommand> mapper) : IRequestHandler<AddTodoDependencyCommand, Guid>
{
    public async Task<Guid> Handle(AddTodoDependencyCommand q, CancellationToken ct)
    {
        var plan = await persistence.DevelopmentPlans.FirstAsync(x => x.UID == q.PlanId, ct);
        var dependency = mapper.ToEntity(q);
        await persistence.AddTodoDependencyAsync(dependency, ct);
        //plan.IncrementDependencyVersion(); // 自动实现
        return dependency.UID;
    }
}

public sealed class RemoveTodoDependencyCommandHandler(IPlanPersistence persistence) : IRequestHandler<RemoveTodoDependencyCommand, Unit>
{
    public async Task<Unit> Handle(RemoveTodoDependencyCommand q, CancellationToken ct)
    {
        TodoDependencyEntity dependencyEntity = await persistence.TodoDependencies.SingleAsync(e => e.UID == q.UID, ct);// 默认没有重复UID 且 于属于 planId 所指的Plan; RemoveTodoDependencyLegalityValidator 业务会保障这点
        persistence.RemoveTodoDependency(dependencyEntity);
        return Unit.Value;
    }
}
