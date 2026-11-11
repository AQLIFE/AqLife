using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.DevelopmentPlan.Validator;

public sealed class ArchiveDevelopmentPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class ArchiveDevelopmentPlanNotDefaultValidator() : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    private protected override string ErrorMessage { init; get; } = "默认开发计划不能归档";
    private protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return command.UID != DevelopmentPlanEntity.DefaultPlanId;
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class ArchiveDevelopmentPlanHasNoActiveTodosValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveDevelopmentPlanCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划仍包含未归档任务，不能归档";
    private protected override async Task<bool> IsValidAsync(ArchiveDevelopmentPlanCommand command, CancellationToken ct)
    {
        return !await db.Todo.AnyAsync(x => x.PlanId == command.UID && !x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
