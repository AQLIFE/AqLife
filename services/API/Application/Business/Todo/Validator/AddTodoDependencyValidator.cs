using AqLife.Application.Validators;
using AqLife.Domain.Command;
using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace AqLife.Application.Business.Todo.Validator;


/// <summary>
/// Todo 存在性 : 前置Todo是否存在
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyPredecessorIdExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "前置任务依赖不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(x => x.UID == command.PredecessorId, ct);
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

/// <summary>
/// Todo 存在性 : 继任者是否存在
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencySuccessorIdExistsValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "继任者任务不存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => await db.Todo.AnyAsync(x => x.UID == command.SuccessorId, ct);
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
/// <summary>
/// 计划归属
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyPlanLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "创建的依赖关系中存在不属于当前计划的任务";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        TodoEntity successor = await db.Todo.SingleOrDefaultAsync(e => e.UID == command.SuccessorId    ,ct ) ?? throw new RequestCheckException("非预期异常,无法执行");
        TodoEntity predecessor = await db.Todo.SingleOrDefaultAsync(e => e.UID == command.PredecessorId,ct) ?? throw new RequestCheckException("非预期异常,无法执行");
        return successor.PlanId == predecessor.PlanId && successor.PlanId == command.PlanId;
    }
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}
/// <summary>
/// 自身依赖
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "目标任务不能是自身的依赖";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => command.PredecessorId != command.SuccessorId;
    private protected override Exception CreateException(string message) => new ResourceNotFoundException(message);
}

/// <summary>
/// 重复依赖
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyRepetitivenessValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "该依赖关系已存在";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    => await db.TodoDependencies.AnyAsync(e => e.PredecessorId == command.PredecessorId && e.SuccessorId == command.SuccessorId, ct);
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}
/// <summary>
/// 归档合法性
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyArchiveLegalityValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "依赖关系中存在归档任务,禁止创建";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        TodoEntity successorTodo =   await db.Todo.SingleOrDefaultAsync(todo => todo.UID == command.SuccessorId,ct) ?? throw new ResourceNotFoundException("不存在的继任Id"); // 当前想要添加依赖的 任务
        TodoEntity predecessorTodo = await db.Todo.SingleOrDefaultAsync(todo => todo.UID == command.PredecessorId,ct) ?? throw new ResourceNotFoundException("不存在的前置Id"); // 即将添加的 继任者
        return !successorTodo.IsArchived && !predecessorTodo.IsArchived;
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
}


/// <summary>
/// 图循环检查 :检查即将创建的依赖是否存在循环引用;疑似可以替代 AddTodoDependencyLegalityValidator
/// </summary>
/// <param name="db"></param>
public sealed class AddTodoDependencyGraphValidator(AqLife.Application.Abstractions.Persistence.IApplicationDbContext db) : AbstractValidator<AddTodoDependencyCommand>
{
    private protected override string ErrorMessage { init; get; } = "禁止创建循环依赖";
    private protected override async Task<bool> IsValidAsync(AddTodoDependencyCommand command, CancellationToken ct)
    {
        //IIncludableQueryable<DevelopmentPlanEntity, IEnumerable<TodoEntity>> todos = db.DevelopmentPlans.Include(e => e.Todos.Where(todo => !todo.IsArchived));//已存在的计划内todo
       
        IDictionary<Guid, Guid[]> Adjacency = await db.TodoDependencies.Where(e => e.PlanId == command.PlanId)
                .GroupBy(x => x.PredecessorId)
                .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.SuccessorId).ToArray(),ct);
        Stack<Guid> Pending = new();    // 存储所有发现的前继任者ID
        HashSet<Guid> Visited = new();  // 存储已经检查过的前继任者ID
        Pending.Push(command.SuccessorId);
        return CanAddDependency(command.PredecessorId,ref Pending ,ref Visited ,Adjacency);
    }
    private protected override Exception CreateException(string message) => new DomainLegalityException(message);
    
    private bool CanAddDependency(in Guid PredecessorId,ref Stack<Guid> pending,ref HashSet<Guid> visited,in IDictionary<Guid, Guid[]> adjacency)
    {
        while (pending.Count > 0)
        {
            var current = pending.Pop();

            if (!visited.Add(current))
                continue;

            if (current == PredecessorId)
                return false;

            if (adjacency.TryGetValue(current, out var successors))
            {
                foreach (var sr in successors)
                    pending.Push(sr);
            }
        }
        return true;
    }
}