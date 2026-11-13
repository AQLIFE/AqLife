using AqLife.Application.Abstractions.Persistence;
using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Contracts.CommandInterface;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class UpdateTodoDependencyCompletionValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoStatusCommand>
{
    private protected override string ErrorMessage { init; get; } = "仍有未完成的前置任务，当前任务不能完成";
    private protected override async Task<bool> IsValidAsync(UpdateTodoStatusCommand command, CancellationToken ct)
        => command.Status == TodoStatus.Completed && await db.Todo.AnyAsync(e => e.UID == command.UID && e.IncomingDependencies.All(de => de.PredecessorTodo.Status == TodoStatus.Completed), ct);
    
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class ArchiveTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务处于未归档状态方可操作";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(e => !e.IsArchived && e.UID == command.UID, ct);
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

// 所属开发计划的归档状态检查 由 DevPlanValidator 覆盖
public sealed class RestoreTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<RestoreTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务处于归档状态方可操作";
    private protected override async Task<bool> IsValidAsync(RestoreTodoCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(e => e.UID == command.UID && e.IsArchived, ct);// 
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}


/// <summary>
/// 解决 B10 问题,任何Command只要继承了 IRequireDevPlan , IRequireValidEntity<TodoEntity>
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="storage"></param>
public class TodoUpdateValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest>
   where TRequest : IRequireDevPlan, IRequireValidEntity<TodoEntity>, IUpdateCommand
{
    private protected override string ErrorMessage { init; get; } = "已归档的任务不可操作";
    private protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
        => command is RestoreTodoCommand || await storage.Todo.AnyAsync(x => x.UID == command.UID && !x.IsArchived, ct);

    private protected override Exception CreateException(string message)
    => new DomainLegalityException(message);
}
/// <summary>
/// 预期更新的todo 不能改变其归属
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <param name="storage"></param>
public class TodoAttributionValidityValidator<TRequest>(IApplicationDbContext storage) : AbstractValidator<TRequest>
   where TRequest : IRequireDevPlan, IRequireValidEntity<TodoEntity>, IUpdateCommand
{
    private protected override string ErrorMessage { init; get; } = "更新操作不能改变任务计划归属";
    private protected override async Task<bool> IsValidAsync(TRequest command, CancellationToken ct)
        => await storage.Todo.AnyAsync(x => x.UID == command.UID && x.PlanId == command.PlanId, ct);

    private protected override Exception CreateException(string message)
    => new DomainLegalityException(message);
}
