using AqLife.Application.Abstractions.Mapper;
using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Business.DevelopmentPlan.Search;
using AqLife.Application.Mappers;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Domain.Services;
using AqLife.Shared.IView;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler;

public sealed class TodoRoadmapQueryHandler(DevelopmentPlanSearch<TodoRoadmapQuery> roadmapSearch,
    IViewMapper<DevelopmentPlanEntity, TodoRoadmapDto> mapper) : IRequestHandler<TodoRoadmapQuery, TodoRoadmapDto>
{
    public async Task<TodoRoadmapDto> Handle(TodoRoadmapQuery q, CancellationToken ct)
    {
        IQueryable<DevelopmentPlanEntity> developmentPlan = await roadmapSearch.SearchAsync(q, ct);

        return mapper.ToDto(await developmentPlan.SingleAsync(ct));// 对于 TodoRoadmapQuery , 只会返回一个 DevelopmentPlanEntity, 因为在 Validator 中已经验证了 PlanId 的合法性
    }
}