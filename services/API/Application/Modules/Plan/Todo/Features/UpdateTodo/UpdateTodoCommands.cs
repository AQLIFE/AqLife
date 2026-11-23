using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Options;
using System.ComponentModel.DataAnnotations;
namespace AqLife.Application.Plan.Todo;
public record UpdateTodoCommand : IRequireValidEntity<TodoEntity>, IRequireDevPlan, IUpdateCommand
{
    public Guid UID { get; init; }
    public Guid PlanId { get; init; }
    [StringLength(500)] public string Title { get; init; } = string.Empty;
    [StringLength(10000)] public string? Description { get; init; }
    [Range(0, int.MaxValue)] public int SortOrder { get; init; } = 0;
}
public record UpdateTodoStatusCommand(Guid UID, TodoStatus Status, Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand, IRequireDevPlan;
public record ArchiveTodoCommand(Guid UID, Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand, IRequireDevPlan;
public record RestoreTodoCommand(Guid UID, Guid PlanId) : IRequireValidEntity<TodoEntity>, IUpdateCommand, IRequireDevPlan;
