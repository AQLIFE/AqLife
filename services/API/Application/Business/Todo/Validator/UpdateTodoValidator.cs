using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

[Obsolete("此验证器已弃用")]
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

public sealed class UpdateTodoDependencyCompletionValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "仍有未完成的前置任务，当前任务不能完成";
    private protected override async Task<bool> IsValidAsync(UpdateTodoCommand command, CancellationToken ct)
        =>await db.Todo.AnyAsync(e => e.UID == command.UID && e.IncomingDependencies.All(de => de.PredecessorTodo.Status == TodoStatus.Completed), ct);
    
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
