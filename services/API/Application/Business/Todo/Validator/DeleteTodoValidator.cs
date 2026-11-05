using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class DeleteTodoExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<DeleteTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "Todo 不存在";
    private protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class DeleteTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<DeleteTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        var planId = await db.Todo.Where(x => x.UID == command.UID).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !planId.HasValue || !await db.DevelopmentPlans.AnyAsync(x => x.UID == planId.Value && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class DeleteTodoNotCompletedValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<DeleteTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已完成任务不能直接删除，请使用归档";
    private protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return !await db.Todo.AnyAsync(x => x.UID == command.UID && x.Status == AqLife.Shared.Options.TodoStatus.Completed, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class DeleteTodoHasNoChildrenValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<DeleteTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "存在子任务，不能删除父任务";
    private protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return !await db.Todo.AnyAsync(x => x.FTID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class DeleteTodoHasNoDependenciesValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<DeleteTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "存在依赖关系，请先移除依赖再删除任务";
    private protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return !await db.TodoDependencies.AnyAsync(x => x.PredecessorId == command.UID || x.SuccessorId == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
