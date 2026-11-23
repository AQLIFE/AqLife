using AqLife.Application.Plan.Todo;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.Todo.Validator;


public sealed class RemoveTodoDependencyLegalityValidator(AqLife.Application.Abstractions.Persistence.Modules.IPlanPersistence persistence) : AbstractValidator<RemoveTodoDependencyCommand>
{
    protected override string ErrorMessage { init; get; } = "不存在该任务依赖";
    protected override async Task<bool> IsValidAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
        => await persistence.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.Todos.Any(e => e.IncomingDependencies.Any(d => d.UID == command.UID) || e.OutgoingDependencies.Any(p => p.UID == command.UID)), ct);

    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
