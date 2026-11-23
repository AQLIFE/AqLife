using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Domain.Entities.DevPlan;
namespace AqLife.Application.Plan.DevelopmentPlan;
public record ArchiveDevelopmentPlanCommand(Guid UID) : IRequireValidEntity<DevelopmentPlanEntity>, IRequireDevPlan, IUpdateCommand
{
    public Guid PlanId { get; init; } = UID;
}
