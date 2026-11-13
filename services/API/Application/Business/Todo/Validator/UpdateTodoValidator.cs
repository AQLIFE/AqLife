using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;


public sealed class UpdateTodoDependencyCompletionValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<UpdateTodoStatusCommand>
{
    private protected override string ErrorMessage { init; get; } = "仍有未完成的前置任务，当前任务不能完成";
    private protected override async Task<bool> IsValidAsync(UpdateTodoStatusCommand command, CancellationToken ct)
        =>await db.Todo.AnyAsync(e => e.UID == command.UID && e.IncomingDependencies.All(de => de.PredecessorTodo.Status == TodoStatus.Completed), ct);
    
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
