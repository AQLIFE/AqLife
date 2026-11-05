using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class AddTodoDependencyPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class AddTodoDependencyPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class AddTodoDependencyGraphValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务依赖关系不合法";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        if (!await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct)) return true;
        var tasks = await db.Todo.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
        var dependencies = await db.TodoDependencies.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
        try
        {
            AqLife.Domain.Services.TodoDependencyGraphValidator.EnsureCanAdd(
                new AqLife.Domain.Entities.TodoDependencyEntity(command.PlanId, command.PredecessorId, command.SuccessorId), tasks, dependencies);
            return true;
        }
        catch (DomainLegalityException)
        {
            return false;
        }
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
