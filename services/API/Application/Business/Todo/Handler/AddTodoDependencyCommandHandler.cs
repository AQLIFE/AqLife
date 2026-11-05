using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Entities; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class AddTodoDependencyCommandHandler(IApplicationDbContext db):IRequestHandler<AddTodoDependencyCommand,Guid>{
public async Task<Guid> Handle(AddTodoDependencyCommand q,CancellationToken ct){var p=await db.DevelopmentPlans.FirstAsync(x=>x.UID==q.PlanId,ct); var e=new TodoDependencyEntity(q.PlanId,q.PredecessorId,q.SuccessorId); await db.TodoDependencies.AddAsync(e,ct); p.IncrementDependencyVersion(); return e.UID;}
}