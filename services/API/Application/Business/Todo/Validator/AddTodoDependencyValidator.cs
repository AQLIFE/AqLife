using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace AqLife.Application.Business.Todo.Validator;

public sealed class AddTodoDependencyPlanExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "开发计划不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    =>await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId, ct);
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);

}

public sealed class AddTodoDependencyPredecessorIdExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "前置任务依赖不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(x => x.UID == command.PredecessorId, ct);
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}


public sealed class AddTodoDependencySuccessorIdExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(x => x.UID == command.SuccessorId, ct);
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

public sealed class AddTodoDependencyPlanLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "创建的依赖关系中存在不属于当前计划的任务";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        TodoEntity successor = await db.Todo.SingleOrDefaultAsync(e => e.UID == command.SuccessorId) ?? throw new RequestCheckException("非预期异常,无法执行");
        TodoEntity predecessor = await db.Todo.SingleOrDefaultAsync(e => e.UID == command.PredecessorId) ?? throw new RequestCheckException("非预期异常,无法执行");
        return successor.PlanId == predecessor.PlanId && successor.PlanId == command.PlanId;
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

public sealed class AddTodoDependencyLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务不能是自身的依赖";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => command.PredecessorId != command.SuccessorId;
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

[Obsolete("使用专属验证器 DevPlanValidator")]
public sealed class AddTodoDependencyPlanActiveValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "已归档的开发计划不允许修改";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        return !await db.DevelopmentPlans.AnyAsync(x => x.UID == command.PlanId && x.IsArchived, ct);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}

