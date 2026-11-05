using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Services; using AqLife.Shared.IView; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class TodoRoadmapQueryHandler(IApplicationDbContext db):IRequestHandler<TodoRoadmapQuery,TodoRoadmapDto>{
public async Task<TodoRoadmapDto> Handle(TodoRoadmapQuery q,CancellationToken ct){
var plan=await db.DevelopmentPlans.AsNoTracking().FirstAsync(x=>x.UID==q.PlanId,ct);
var tasks=await db.Todo.AsNoTracking().Where(x=>x.PlanId==q.PlanId&&!x.IsArchived).OrderBy(x=>x.SortOrder).ThenBy(x=>x.CreatedAt).ToListAsync(ct);
var deps=await db.TodoDependencies.AsNoTracking().Where(x=>x.PlanId==q.PlanId).ToListAsync(ct);var ids=tasks.Select(x=>x.UID).ToArray();
var layouts=await db.RoadmapNodeLayouts.AsNoTracking().Where(x=>x.PlanId==q.PlanId&&ids.Contains(x.TodoId)).ToListAsync(ct);var byId=tasks.ToDictionary(x=>x.UID);
var nodes=tasks.Select(t=>{var s=TodoDependencyStateCalculator.Calculate(t,deps,byId);return new TodoDto(t.UID,t.FTID,t.Title,t.Status.ToString(),t.CreatedAt,t.CompletedAt,t.Priority,new List<TodoDto>(),false){PlanId=t.PlanId,Title=t.Title,Description=t.Description,UpdatedAt=t.UpdatedAt,SortOrder=t.SortOrder,IsArchived=t.IsArchived,IsBlocked=s.IsBlocked,IncompleteDependencyCount=s.IncompleteDependencyCount,BlockingTodoIds=s.BlockingTodoIds};}).ToList();
return new TodoRoadmapDto(plan.UID,plan.Name,plan.DependencyVersion,nodes,deps.Select(x=>new TodoDependencyDto(x.PredecessorId,x.SuccessorId,x.CreatedAt)).ToList(),layouts.Select(x=>new RoadmapNodeLayoutDto(x.TodoId,x.X,x.Y,x.UpdatedAt)).ToList());
}}