using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Plan.Contracts;
using AqLife.Application.Plan.Todo;
using AqLife.Domain.Entities.DevPlan;
namespace AqLife.Application.Plan.DevelopmentPlan;
public record DevelopmentPlanQuery(Guid? UID = null, string? Title = null) : IQuery<IReadOnlyList<DevelopmentPlanDto>>, IReadDevPlan;
public record TodoRoadmapQuery(Guid PlanId) : IRequireValidEntity<DevelopmentPlanEntity>, IQuery<TodoRoadmapDto>, IReadDevPlan
{
    /// <summary>与 PlanId 相同，仅用于满足 IRequireValidEntity 依赖</summary>
    public Guid UID { get; init; } = PlanId;
}
