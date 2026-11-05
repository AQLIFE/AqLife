using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class SetTodoParentExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SetTodoParentCommand>
{
    private protected override string ErrorMessage { init; get; } = "Todo 不存在";
    private protected override async Task<bool> IsValidAsync(SetTodoParentCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class SetTodoParentPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SetTodoParentCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(SetTodoParentCommand command, CancellationToken ct)
    {
        var planId = await db.Todo.Where(x => x.UID == command.UID).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !planId.HasValue || !await db.DevelopmentPlans.AnyAsync(x => x.UID == planId.Value && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class SetTodoParentHierarchyValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SetTodoParentCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务层级调整不合法";
    private protected override async Task<bool> IsValidAsync(SetTodoParentCommand command, CancellationToken ct)
    {
        var todo = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct);
        if (todo is null) return true;
        var tasks = await db.Todo.Where(x => x.PlanId == todo.PlanId).ToListAsync(ct);
        try
        {
            AqLife.Domain.Services.TodoHierarchyValidator.EnsureCanMove(todo, command.ParentId, tasks);
            return true;
        }
        catch (DomainLegalityException)
        {
            return false;
        }
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
