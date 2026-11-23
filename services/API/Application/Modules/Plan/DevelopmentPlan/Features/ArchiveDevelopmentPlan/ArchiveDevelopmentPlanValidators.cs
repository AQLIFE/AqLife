using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.DevelopmentPlan.Validator;

public sealed class ArchiveDevelopmentPlanExistsValidator(Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划不存在";
    protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.UID, ct);
    }
    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class ArchiveDevelopmentPlanNotDefaultValidator() : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "默认开发计划不能归档";
    protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return command.UID != DevelopmentPlanEntity.DefaultPlanId;
    }
    protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class ArchiveDevelopmentPlanHasNoActiveTodosValidator(Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划仍包含未归档任务，不能归档";
    protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return !await db.Todo.AnyAsync(x => x.PlanId == command.UID && !x.IsArchived, ct);
    }
    protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
