using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Todo;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class AddTodoDependencyCommandHandler(
    IApplicationDbContext db,
    TodoMapper mapper) : IRequestHandler<AddTodoDependencyCommand, Guid>
{
    public async Task<Guid> Handle(AddTodoDependencyCommand q, CancellationToken ct)
    {
        var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == q.PlanId, ct);
        var dependency = mapper.ToEntity(q);
        await db.TodoDependencies.AddAsync(dependency, ct);
        plan.IncrementDependencyVersion();
        return dependency.UID;
    }
}