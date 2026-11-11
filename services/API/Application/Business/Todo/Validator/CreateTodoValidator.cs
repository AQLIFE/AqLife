using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class CreateTodoPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}
[Obsolete("使用专属验证器 DevPlanValidator 替代")]
public sealed class CreateTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<CreateTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(CreateTodoCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
