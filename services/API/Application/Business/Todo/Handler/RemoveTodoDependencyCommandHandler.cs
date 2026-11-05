using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Entities; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class RemoveTodoDependencyCommandHandler(IApplicationDbContext db):IRequestHandler<RemoveTodoDependencyCommand,Guid>{
public async Task<Guid> Handle(RemoveTodoDependencyCommand q,CancellationToken ct){var p=await db.DevelopmentPlans.FirstAsync(x=>x.UID==q.PlanId,ct); var e=await db.TodoDependencies.FirstAsync(x=>x.PlanId==q.PlanId&&x.PredecessorId==q.PredecessorId&&x.SuccessorId==q.SuccessorId,ct); db.TodoDependencies.Remove(e); p.IncrementDependencyVersion(); return e.UID;}
}