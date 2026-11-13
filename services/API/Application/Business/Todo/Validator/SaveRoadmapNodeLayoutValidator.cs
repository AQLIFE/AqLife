using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;


public sealed class SaveRoadmapLayoutTodoInPlanValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "任务不存在或不属于当前计划";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.TodoId && x.PlanId == command.PlanId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

public sealed class SaveRoadmapLayoutPlanConsistencyValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务已存档,不允许改变其布局";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
        =>await db.Todo.AnyAsync(e => e.UID == command.TodoId && !e.IsArchived, ct);    
    
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
