using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
using Plan.Todo;
namespace Plan.Todo.Validator;

public sealed class CreateTodoPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "开发计划不存在";
    protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    => await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}


public sealed class CreateTodoDuplicateNameValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "计划名称不能重复";
    protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    => await db.DevelopmentPlans.AllAsync(x => x.Name != command.Title, ct);
    protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}
