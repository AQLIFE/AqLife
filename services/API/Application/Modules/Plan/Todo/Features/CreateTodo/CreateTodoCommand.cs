using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Plan.Todo;
/// <summary>添加任务：自动添加布局并设置所属计划</summary>
public record CreateTodoCommand : ICreateCommand, IRequireDevPlan
{
    public Guid PlanId { get; init; } = DevelopmentPlanEntity.DefaultPlanId;
    [StringLength(500, ErrorMessage = "任务标题不能超过 500 个字符")] public string Title { get; init; } = string.Empty;
    [StringLength(10000, ErrorMessage = "任务描述长度不能超过 10000 个字符")] public string? Description { get; init; }
    [Range(0, int.MaxValue)] public int SortOrder { get; init; } = 0;
}
