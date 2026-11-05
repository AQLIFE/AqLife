using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class ArchiveTodoExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "Todo 不存在";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class ArchiveTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    {
        var planId = await db.Todo.Where(x => x.UID == command.UID).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !planId.HasValue || !await db.DevelopmentPlans.AnyAsync(x => x.UID == planId.Value && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class ArchiveTodoHasNoChildrenValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "请先处理子任务，再归档该任务";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    {
        return !await db.Todo.AnyAsync(x => x.FTID == command.UID && !x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class ArchiveTodoHasNoDependenciesValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "请先移除任务依赖关系，再归档该任务";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    {
        return !await db.TodoDependencies.AnyAsync(x => x.PredecessorId == command.UID || x.SuccessorId == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
