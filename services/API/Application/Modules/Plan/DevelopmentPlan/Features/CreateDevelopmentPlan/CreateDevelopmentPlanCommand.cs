using AqLife.Application.Abstractions.Commands;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Plan.DevelopmentPlan;
public record CreateDevelopmentPlanCommand(
    [Required, StringLength(128, MinimumLength = 1)] string Name,
    [StringLength(2000)] string? Description = null) : ICreateCommand;
