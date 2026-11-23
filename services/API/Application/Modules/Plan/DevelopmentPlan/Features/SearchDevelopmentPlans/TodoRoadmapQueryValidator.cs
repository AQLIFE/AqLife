using AqLife.Application.Plan.DevelopmentPlan;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Plan.Todo.Validator;

public sealed class TodoRoadmapPlanAvailableValidator(Abstractions.Persistence.IApplicationDbContext db)
    : AbstractValidator<TodoRoadmapQuery>
{
    protected override string ErrorMessage { init; get; } = "开发计划不存在";

    protected override async Task<bool> IsValidAsync(TodoRoadmapQuery command, CancellationToken ct)
        => await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && !x.IsArchived, ct);

    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
