using AqLife.Application.Plan.Todo;
using AqLife.Shared.Exceptions;
using Core.Validators;
using Microsoft.EntityFrameworkCore;
namespace AqLife.Application.Plan.Todo.Validator;

public sealed class DeleteTodoNotCompletedValidator(AqLife.Application.Abstractions.Persistence.Modules.IPlanPersistence persistence) : AbstractValidator<DeleteTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "已完成任务不能直接删除，请使用归档";
    protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return !await persistence.Todos.AnyAsync(x => x.UID == command.UID && x.Status == AqLife.Shared.Options.TodoStatus.Completed, ct);
    }
    protected override Exception CreateException(string message) => new DomainLegalityException(message);

}

public sealed class DeleteTodoHasNoDependenciesValidator(AqLife.Application.Abstractions.Persistence.Modules.IPlanPersistence persistence) : AbstractValidator<DeleteTodoCommand>
{
    protected override string ErrorMessage { init; get; } = "存在依赖关系，请先移除依赖再删除任务";
    protected override async Task<bool> IsValidAsync(DeleteTodoCommand command, CancellationToken ct)
    {
        return !await persistence.TodoDependencies.AnyAsync(x => x.PredecessorId == command.UID || x.SuccessorId == command.UID, ct);
    }
    protected override Exception CreateException(string message) => new DomainLegalityException(message);

}
