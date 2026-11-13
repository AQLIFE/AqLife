using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;


public sealed class RemoveTodoDependencyLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<RemoveTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "不存在该任务依赖";
    private protected override async Task<bool> IsValidAsync(RemoveTodoDependencyCommand command, CancellationToken ct)
        => await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.Todos.Any(e=>e.IncomingDependencies.Any(d=>d.UID == command.UID) || e.OutgoingDependencies.Any(p=>p.UID == command.UID)), ct);
    
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
