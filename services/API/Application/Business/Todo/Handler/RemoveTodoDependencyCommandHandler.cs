using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;

public sealed class RemoveTodoDependencyCommandHandler(IApplicationDbContext db) : IRequestHandler<RemoveTodoDependencyCommand, Unit>
{
    public async Task<Unit> Handle(RemoveTodoDependencyCommand q, CancellationToken ct)
    {
        TodoDependencyEntity dependencyEntity = await db.TodoDependencies.SingleAsync(e => e.UID == q.UID, ct);// 默认没有重复UID 且 于属于 planId 所指的Plan; RemoveTodoDependencyLegalityValidator 业务会保障这点
        db.TodoDependencies.Remove(dependencyEntity);
        return Unit.Value;
    }
}
