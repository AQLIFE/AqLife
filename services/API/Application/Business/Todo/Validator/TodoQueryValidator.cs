using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class TodoQueryPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<TodoQuery>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(TodoQuery command, CancellationToken ct)
    {
        var planId = command.PlanId ?? AqLife.Domain.Entities.DevelopmentPlanEntity.DefaultPlanId;
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == planId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}
