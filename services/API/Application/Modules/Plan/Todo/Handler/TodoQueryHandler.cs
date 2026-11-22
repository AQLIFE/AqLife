using AqLife.Application.Abstractions.Mapping;
using AqLife.Application.Abstractions.Search;
using AqLife.Domain.Entities.DevPlan;
using Core.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Plan.DevelopmentPlan;
using Plan.Todo;

namespace Plan.Todo.Handler;

public sealed class TodoQueryHandler(
    ISearch<TodoQuery, PageResult<TodoDto>, TodoEntity> todoSearch,
    PageResultMapper<TodoEntity, TodoDto> pageMapper) : IRequestHandler<TodoQuery, PageResult<TodoDto>>
{
    public async Task<PageResult<TodoDto>> Handle(TodoQuery q, CancellationToken ct)
    {
        PageResult<TodoEntity> result = await todoSearch.SearchPageAsync(q, ct);
        return pageMapper.ToDto(result);
    }
}

public sealed class TodoRoadmapQueryHandler(ISearch<TodoRoadmapQuery, TodoRoadmapDto, DevelopmentPlanEntity> roadmapSearch,
    IViewMapper<DevelopmentPlanEntity, TodoRoadmapDto> mapper) : IRequestHandler<TodoRoadmapQuery, TodoRoadmapDto>
{
    public async Task<TodoRoadmapDto> Handle(TodoRoadmapQuery q, CancellationToken ct)
    {
        IQueryable<DevelopmentPlanEntity> developmentPlan = await roadmapSearch.SearchAsync(q, ct);

        return mapper.ToDto(await developmentPlan.SingleAsync(ct));// 对于 TodoRoadmapQuery , 只会返回一个 DevelopmentPlanEntity, 因为在 Validator 中已经验证了 PlanId 的合法性
    }
}