using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;


public sealed class RemoveTodoDependencyLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<RemoveTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在该任务";
    private protected override async Task<bool> IsValidAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
        => await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.Todos.Any(e=>e.IncomingDependencies.Any(d=>d.UID == command.UID) || e.OutgoingDependencies.Any(p=>p.UID == command.UID)), ct);
    
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

[Obsolete("此验证器已弃用")]
public sealed class RemoveTodoDependencyPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<RemoveTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
