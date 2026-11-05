using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Todo;
using AqLife.Domain.Command;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class SaveRoadmapNodeLayoutCommandHandler(
    IApplicationDbContext db,
    TodoMapper mapper) : IRequestHandler<SaveRoadmapNodeLayoutCommand, Guid>
{
    public async Task<Guid> Handle(SaveRoadmapNodeLayoutCommand q, CancellationToken ct)
    {
        var layout = await db.RoadmapNodeLayouts.FirstOrDefaultAsync(x => x.TodoId == q.TodoId, ct);
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