using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Entities; using AqLife.Domain.Services; using AqLife.Shared.IView; using AqLife.Shared.Options; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class TodoQueryHandler(IApplicationDbContext db):IRequestHandler<TodoQuery,PageResult<TodoDto>>{
public async Task<PageResult<TodoDto>> Handle(TodoQuery q,CancellationToken ct){
var planId=q.PlanId??DevelopmentPlanEntity.DefaultPlanId;
var all=await db.Todo.AsNoTracking().Where(x=>x.PlanId==planId).OrderBy(x=>x.SortOrder).ThenBy(x=>x.CreatedAt).ToListAsync(ct);
var deps=await db.TodoDependencies.AsNoTracking().Where(x=>x.PlanId==planId).ToListAsync(ct); var byId=all.ToDictionary(x=>x.UID);
var states=all.ToDictionary(x=>x.UID,x=>TodoDependencyStateCalculator.Calculate(x,deps,byId));
var visible=all.AsEnumerable(); if(q.UID.HasValue)visible=visible.Where(x=>x.UID==q.UID.Value);
if(!string.IsNullOrWhiteSpace(q.Desc))visible=visible.Where(x=>x.Title.Contains(q.Desc,StringComparison.OrdinalIgnoreCase));
if(!q.IncludeCompleted)visible=visible.Where(x=>x.Status!=TodoStatus.Completed); if(!q.IncludeArchived)visible=visible.Where(x=>!x.IsArchived);
if(q.ParentId.HasValue)visible=visible.Where(x=>x.FTID==q.ParentId.Value); else if(q.IsTree&&!q.UID.HasValue)visible=visible.Where(x=>x.FTID==null);
var list=visible.ToList();var page=Math.Max(1,q.Page);var size=Math.Clamp(q.PageSize,1,500);var more=list.Count>page*size;
TodoDto Map(TodoEntity e,bool children){var cs=children?all.Where(x=>x.FTID==e.UID).Where(x=>q.IncludeArchived||!x.IsArchived).Where(x=>q.IncludeCompleted||x.Status!=TodoStatus.Completed).OrderBy(x=>x.SortOrder).ThenBy(x=>x.CreatedAt).Select(x=>Map(x,true)).ToList():new List<TodoDto>();var s=states[e.UID];return new TodoDto(e.UID,e.FTID,e.Title,e.Status.ToString(),e.CreatedAt,e.CompletedAt,e.Priority,cs,cs.Count>0){PlanId=e.PlanId,Title=e.Title,Description=e.Description,UpdatedAt=e.UpdatedAt,SortOrder=e.SortOrder,IsArchived=e.IsArchived,IsBlocked=s.IsBlocked,IncompleteDependencyCount=s.IncompleteDependencyCount,BlockingTodoIds=s.BlockingTodoIds};}
return new PageResult<TodoDto>(list.Skip((page-1)*size).Take(size).Select(x=>Map(x,q.IsTree)).ToList(),page,size,more);
}}