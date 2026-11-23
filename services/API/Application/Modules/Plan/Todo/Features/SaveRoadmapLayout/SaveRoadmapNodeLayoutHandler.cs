using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Plan.Todo;
using AqLife.Domain.Entities.DevPlan;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.Todo.Handler;

public sealed class SaveRoadmapNodeLayoutCommandHandler(
    IApplicationDbContext db,
    IBaseEntityMapper<RoadmapNodeLayoutEntity, SaveRoadmapNodeLayoutCommand> mapper) : IRequestHandler<SaveRoadmapNodeLayoutCommand, Guid>
{
    public async Task<Guid> Handle(SaveRoadmapNodeLayoutCommand q, CancellationToken ct)
    {
        var layout = await db.RoadmapNodeLayouts.SingleOrDefaultAsync(x => x.UID == q.TodoId, ct);
        if (layout is null)
        {
            layout = mapper.ToEntity(q);
            await db.RoadmapNodeLayouts.AddAsync(layout, ct);
        }
        else
        {
            layout.SetPosition(q.X, q.Y);
        }

        return layout.UID;
    }
}