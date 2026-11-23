using AqLife.Application.Abstractions.Pipeline.Markers;
using AqLife.Application.Abstractions.Commands;
using AqLife.Application.Abstractions.Persistence.Modules;
using AqLife.Application.Plan.Contracts;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
using AqLife.Application.Plan.Todo;

namespace AqLife.Application.Plan.Todo.Validator;

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class UpdateTodoDependencyCompletionValidator(IPlanPersistence persistence) : AbstractValidator<UpdateTodoStatusCommand>
{
    protected override string ErrorMessage { init; get; } = "仍有未完成的前置任务，当前任务不能完成";
    protected override async Task<bool> IsValidAsync(UpdateTodoStatusCommand command, CancellationToken ct)
        => command.Status == TodoStatus.Completed && await persistence.Todos.AnyAsync(e => e.UID == command.UID && e.IncomingDependencies.All(de => de.PredecessorTodo.Status == TodoStatus.Completed), ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class ArchiveTodoPlanActiveValidator(IPlanPersistence persistence) : AbstractValidator<ArchiveTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "目标任务处于未归档状态方可操作";
    protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
        => await persistence.Todos.AnyAsync(e => !e.IsArchived && e.UID == command.UID, ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class RestoreTodoPlanActiveValidator(IPlanPersistence persistence) : AbstractValidator<RestoreTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "目标任务处于归档状态方可操作";
    protected override async Task<bool> IsValidAsync(RestoreTodoCommand command, CancellationToken ct)
        => await persistence.Todos.AnyAsync(e => e.UID == command.UID && e.IsArchived, ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

/// <summary>
/// 解决 B10 问题：实现 IRequireDevPlan、IRequireValidEntity 和 IUpdateCommand 的请求统一验证。
/// </summary>
public class TodoUpdateValidator<TRequest>(IPlanPersistence persistence) : AbstractValidator<TRequest>
    where TRequest : IRequireDevPlan, IRequireValidEntity<TodoEntity>, IUpdateCommand
{
    protected override string ErrorMessage { init; get; } = "已归档的任务不可操作";
    protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
        => command is RestoreTodoCommand || await persistence.Todos.AnyAsync(x => x.UID == command.UID && !x.IsArchived, ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

/// <summary>
/// 预期更新的 Todo 不能改变其归属。
/// </summary>
public class TodoAttributionValidityValidator<TRequest>(IPlanPersistence persistence) : AbstractValidator<TRequest>
    where TRequest : IRequireDevPlan, IRequireValidEntity<TodoEntity>, IUpdateCommand
{
    protected override string ErrorMessage { init; get; } = "更新操作不能改变任务计划归属";
    protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
        => await persistence.Todos.AnyAsync(x => x.UID == command.UID && x.PlanId == command.PlanId, ct);
    protected override Exception CreateException(string message) => new DomainLegalityException(message);
}
