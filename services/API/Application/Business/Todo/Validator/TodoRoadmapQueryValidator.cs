using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Validator;

public sealed class TodoRoadmapPlanAvailableValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db)
    : AbstractValidator<TodoRoadmapQuery>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";

    private protected override async Task<bool> IsValidAsync(TodoRoadmapQuery command, CancellationToken ct)
        => await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && !x.IsArchived, ct);

    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
