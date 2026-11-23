using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Plan.DevelopmentPlan;
public record UpdateDevelopmentPlanCommand(
    Guid UID,
    [Required, StringLength(128, MinimumLength = 1)] string Name,
    [StringLength(2000)] string? Description = null) : IRequireValidEntity<DevelopmentPlanEntity>, IRequireDevPlan, IUpdateCommand
{
    public Guid PlanId { get; init; } = UID;
}
