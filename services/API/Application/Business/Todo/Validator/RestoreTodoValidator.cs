using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class RestoreTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<RestoreTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(RestoreTodoCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(e => !e.Plan.IsArchived && e.IsArchived, ct);// 仅允许未存档的开发计划且任务自身处于存档状态
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}
