using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class SetTodoSortOrderExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SetTodoSortOrderCommand>
{
    private protected override string ErrorMessage { init; get; } = "Todo 不存在";
    private protected override async Task<bool> IsValidAsync(SetTodoSortOrderCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.UID, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class SetTodoSortOrderPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SetTodoSortOrderCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(SetTodoSortOrderCommand command, CancellationToken ct)
    {
        var planId = await db.Todo.Where(x => x.UID == command.UID).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !planId.HasValue || !await db.DevelopmentPlans.AnyAsync(x => x.UID == planId.Value && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
