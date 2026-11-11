using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

[Obsolete("使用专属验证器 DevPlanValidator 替代")]
public sealed class ArchiveTodoPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<ArchiveTodoCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(ArchiveTodoCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(e => !e.Plan.IsArchived && !e.IsArchived, ct);// 仅允许未存档的开发计划且任务自身 也处于未存档状态
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}
