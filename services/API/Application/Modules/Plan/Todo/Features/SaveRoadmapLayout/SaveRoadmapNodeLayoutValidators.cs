using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Todo;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.Todo.Validator;

public sealed class SaveRoadmapLayoutTodoInPlanValidator(IPlanPersistence persistence)
    : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    protected override string ErrorMessage { init; get; } = "任务不存在或不属于当前计划";
    protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
        => await persistence.Todos.AnyAsync(x => x.UID == command.TodoId && x.PlanId == command.PlanId, ct);
    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

public sealed class SaveRoadmapLayoutPlanConsistencyValidator(IPlanPersistence persistence)
    : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    protected override string ErrorMessage { init; get; } = "目标任务已存档,不允许改变其布局";
    protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
        => await persistence.Todos.AnyAsync(e => e.UID == command.TodoId && !e.IsArchived, ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}
