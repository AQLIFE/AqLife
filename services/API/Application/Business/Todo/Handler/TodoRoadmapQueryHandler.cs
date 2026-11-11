using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
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
        DevelopmentPlanEntity developmentPlan = await db.DevelopmentPlans.AsNoTracking()
            .Where(e => e.UID == q.PlanId).Include(e => e.Todos).ThenInclude(todo => todo.LayoutEntity)
            .Include(e => e.Todos).ThenInclude(todo => todo.IncomingDependencies)
            .Include(e => e.Todos).ThenInclude(todo => todo.OutgoingDependencies)
            .AsSplitQuery().SingleAsync(ct);

        return mapper.ToDto(developmentPlan);
    }
}