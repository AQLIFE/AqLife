using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class UpdateTodoExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "Todo 不存在";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class UpdateTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
    {
        var planId = await db.Todo.Where(x => x.UID == command.UID).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !planId.HasValue || !await db.DevelopmentPlans.AnyAsync(x => x.UID == planId.Value && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class UpdateTodoTitleValidator() : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务标题不能为空且不能超过 500 个字符";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
    {
        return !string.IsNullOrWhiteSpace(command.EffectiveTitle) && command.EffectiveTitle.Trim().Length <= 500;
    }

}

public sealed class UpdateTodoStatusValidator() : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "无效的任务状态";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
    {
        return Enum.IsDefined(command.Status);
    }

}

public sealed class UpdateTodoDependencyCompletionValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "仍有未完成的前置任务，当前任务不能完成";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
    {
        var current = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct);
        if (current is null || command.Status != AqLife.Shared.Options.TodoStatus.Completed || current.Status == AqLife.Shared.Options.TodoStatus.Completed) return true;
        return !await db.TodoDependencies
            .Where(x => x.PlanId == current.PlanId && x.SuccessorId == current.UID)
            .Join(db.Todo, dependency => dependency.PredecessorId, todo => todo.UID, (_, todo) => todo)
            .AnyAsync(todo => todo.Status != AqLife.Shared.Options.TodoStatus.Completed, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
