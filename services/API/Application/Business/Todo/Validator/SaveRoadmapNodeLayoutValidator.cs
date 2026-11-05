using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class SaveRoadmapLayoutPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
    {
        return await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class SaveRoadmapLayoutPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class SaveRoadmapLayoutTodoInPlanValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "科技树节点不存在或不属于当前计划";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
    {
        return await db.Todo.AnyAsync(x => x.UID == command.TodoId && x.PlanId == command.PlanId && !x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class SaveRoadmapLayoutPlanConsistencyValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<SaveRoadmapNodeLayoutCommand>
{
    private protected override string ErrorMessage { init; get; } = "布局与任务所属计划不一致";
    private protected override async Task<bool> IsValidAsync(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
    {
        var layoutPlanId = await db.RoadmapNodeLayouts.Where(x => x.TodoId == command.TodoId).Select(x => (Guid?)x.PlanId).FirstOrDefaultAsync(ct);
        return !layoutPlanId.HasValue || layoutPlanId.Value == command.PlanId;
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
