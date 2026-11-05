using AqLife.Application.Abstractions.Persistence; using AqLife.Domain.Command; using AqLife.Domain.Entities; using MediatR; using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Handler;
public sealed class SaveRoadmapNodeLayoutCommandHandler(IApplicationDbContext db):IRequestHandler<SaveRoadmapNodeLayoutCommand,Guid>{
public async Task<Guid> Handle(SaveRoadmapNodeLayoutCommand q,CancellationToken ct){var l=await db.RoadmapNodeLayouts.FirstOrDefaultAsync(x=>x.TodoId==q.TodoId,ct); if(l is null){l=new RoadmapNodeLayoutEntity(q.PlanId,q.TodoId,q.X,q.Y);await db.RoadmapNodeLayouts.AddAsync(l,ct);}else l.SetPosition(q.X,q.Y);return l.UID;}
}