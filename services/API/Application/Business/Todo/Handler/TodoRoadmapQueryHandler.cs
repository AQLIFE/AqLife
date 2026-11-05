using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.Todo;
using AqLife.Domain.Command;
using AqLife.Domain.Services;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class TodoRoadmapQueryHandler(
    IApplicationDbContext db,
    TodoMapper mapper) : IRequestHandler<TodoRoadmapQuery, TodoRoadmapDto>
{
    public async Task<TodoRoadmapDto> Handle(TodoRoadmapQuery q, CancellationToken ct)
    {
        var plan = await db.DevelopmentPlans.AsNoTracking()
            .FirstAsync(x => x.UID == q.PlanId, ct);
        var tasks = await db.Todo.AsNoTracking()
            .Where(x => x.PlanId == q.PlanId && !x.IsArchived)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(ct);
        var dependencies = await db.TodoDependencies.AsNoTracking()
            .Where(x => x.PlanId == q.PlanId)
            .ToListAsync(ct);
        var ids = tasks.Select(x => x.UID).ToArray();
        var layouts = await db.RoadmapNodeLayouts.AsNoTracking()
            .Where(x => x.PlanId == q.PlanId && ids.Contains(x.TodoId))
            .ToListAsync(ct);

        var byId = tasks.ToDictionary(x => x.UID);
        var nodes = tasks.Select(task =>
        {
            var state = TodoDependencyStateCalculator.Calculate(task, dependencies, byId);
            return mapper.ToDto(task, state);
        }).ToList();

        return mapper.ToRoadmapDto(
            plan,
            nodes,
            dependencies.Select(mapper.ToDto).ToList(),
            layouts.Select(mapper.ToDto).ToList());
    }
}