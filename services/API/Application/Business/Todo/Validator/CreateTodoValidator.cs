using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class CreateTodoPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class CreateTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class CreateTodoTitleValidator() : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务标题不能为空且不能超过 500 个字符";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        return !string.IsNullOrWhiteSpace(command.EffectiveTitle) && command.EffectiveTitle.Trim().Length <= 500;
    }

}

public sealed class CreateTodoHierarchyValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务父子层级不合法";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        if (!await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct)) return true;
        var tasks = await db.Todo.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
        var todo = new AqLife.Domain.Entities.TodoEntity(command.PlanId, command.EffectiveTitle, command.Description, command.Priority, command.EffectiveParentId, command.SortOrder);
        try
        {
            AqLife.Domain.Services.TodoHierarchyValidator.EnsureCanMove(todo, command.EffectiveParentId, tasks.Append(todo).ToArray());
            return true;
        }
        catch (DomainLegalityException)
        {
            return false;
        }
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
