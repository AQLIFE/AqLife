using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

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