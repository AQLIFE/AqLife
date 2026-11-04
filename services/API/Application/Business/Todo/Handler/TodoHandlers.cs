using AqLife.Application.Abstractions.Persistence;
using AqLife.Domain.Command;
using AqLife.Domain.Entities;
using AqLife.Domain.Services;
using AqLife.Shared.Exceptions;
using AqLife.Shared.IView;
using AqLife.Shared.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AqLife.Application.Business.Todo.Handler
{
    public sealed class TodoQueryHandler(IApplicationDbContext db)
        : IRequestHandler<TodoQuery, PageResult<TodoDto>>
    {
        public async Task<PageResult<TodoDto>> Handle(TodoQuery query, CancellationToken ct)
        {
            var planId = query.PlanId ?? DevelopmentPlanEntity.DefaultPlanId;
            if (!await db.DevelopmentPlans.AnyAsync(x => x.UID == planId, ct))
                throw new ResourceNotFoundException("开发计划不存在");

            var allPlanTasks = await db.Todo.AsNoTracking()
                .Where(x => x.PlanId == planId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.CreatedAt)
                .ToListAsync(ct);
            var dependencies = await db.TodoDependencies.AsNoTracking()
                .Where(x => x.PlanId == planId).ToListAsync(ct);
            var byId = allPlanTasks.ToDictionary(x => x.UID);
            var states = allPlanTasks.ToDictionary(x => x.UID,
                x => TodoDependencyStateCalculator.Calculate(x, dependencies, byId));

            var visible = allPlanTasks.AsEnumerable();
            if (query.UID.HasValue) visible = visible.Where(x => x.UID == query.UID.Value);
            if (!string.IsNullOrWhiteSpace(query.Desc))
                visible = visible.Where(x => x.Title.Contains(query.Desc, StringComparison.OrdinalIgnoreCase));
            if (!query.IncludeCompleted) visible = visible.Where(x => x.Status != TodoStatus.Completed);
            if (!query.IncludeArchived) visible = visible.Where(x => !x.IsArchived);
            if (query.ParentId.HasValue) visible = visible.Where(x => x.FTID == query.ParentId.Value);
            else if (query.IsTree && !query.UID.HasValue) visible = visible.Where(x => x.FTID == null);

            var visibleTasks = visible.ToList();
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 500);
            var hasMore = visibleTasks.Count > page * pageSize;
            var pageItems = visibleTasks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            TodoDto Map(TodoEntity entity, bool includeChildren)
            {
                var children = includeChildren
                    ? allPlanTasks.Where(x => x.FTID == entity.UID)
                        .Where(x => query.IncludeArchived || !x.IsArchived)
                        .Where(x => query.IncludeCompleted || x.Status != TodoStatus.Completed)
                        .OrderBy(x => x.SortOrder).ThenBy(x => x.CreatedAt)
                        .Select(x => Map(x, true)).ToList()
                    : new List<TodoDto>();
                var state = states[entity.UID];
                return new TodoDto(entity.UID, entity.FTID, entity.Title, entity.Status.ToString(),
                    entity.CreatedAt, entity.CompletedAt, entity.Priority, children, children.Count > 0)
                {
                    PlanId = entity.PlanId,
                    Title = entity.Title,
                    Description = entity.Description,
                    UpdatedAt = entity.UpdatedAt,
                    SortOrder = entity.SortOrder,
                    IsArchived = entity.IsArchived,
                    IsBlocked = state.IsBlocked,
                    IncompleteDependencyCount = state.IncompleteDependencyCount,
                    BlockingTodoIds = state.BlockingTodoIds
                };
            }

            return new PageResult<TodoDto>(
                pageItems.Select(x => Map(x, query.IsTree)).ToList(), page, pageSize, hasMore);
        }
    }

    public sealed class TodoRoadmapQueryHandler(IApplicationDbContext db)
        : IRequestHandler<TodoRoadmapQuery, TodoRoadmapDto>
    {
        public async Task<TodoRoadmapDto> Handle(TodoRoadmapQuery query, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UID == query.PlanId, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");

            if (plan.IsArchived)
                throw new ResourceNotFoundException("开发计划不存在");

            var tasks = await db.Todo.AsNoTracking().Where(x => x.PlanId == query.PlanId && !x.IsArchived)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.CreatedAt).ToListAsync(ct);
            var dependencies = await db.TodoDependencies.AsNoTracking()
                .Where(x => x.PlanId == query.PlanId).ToListAsync(ct);
            var activeNodeIds = tasks.Select(x => x.UID).ToArray();
            var layouts = await db.RoadmapNodeLayouts.AsNoTracking()
                .Where(x => x.PlanId == query.PlanId && activeNodeIds.Contains(x.TodoId)).ToListAsync(ct);
            var byId = tasks.ToDictionary(x => x.UID);
            var nodes = tasks.Select(task =>
            {
                var state = TodoDependencyStateCalculator.Calculate(task, dependencies, byId);
                return new TodoDto(task.UID, task.FTID, task.Title, task.Status.ToString(),
                    task.CreatedAt, task.CompletedAt, task.Priority, new List<TodoDto>(), false)
                {
                    PlanId = task.PlanId,
                    Title = task.Title,
                    Description = task.Description,
                    UpdatedAt = task.UpdatedAt,
                    SortOrder = task.SortOrder,
                    IsArchived = task.IsArchived,
                    IsBlocked = state.IsBlocked,
                    IncompleteDependencyCount = state.IncompleteDependencyCount,
                    BlockingTodoIds = state.BlockingTodoIds
                };
            }).ToList();

            return new TodoRoadmapDto(
                plan.UID, plan.Name, plan.DependencyVersion, nodes,
                dependencies.Select(x => new TodoDependencyDto(x.PredecessorId, x.SuccessorId, x.CreatedAt)).ToList(),
                layouts.Select(x => new RoadmapNodeLayoutDto(x.TodoId, x.X, x.Y, x.UpdatedAt)).ToList());
        }
    }

    public sealed class CreateTodoCommandHandler(IApplicationDbContext db)
        : IRequestHandler<CreateTodoCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.PlanId, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            plan.EnsureActive();

            var task = new TodoEntity(command.PlanId, command.EffectiveTitle, command.Description,
                command.Priority, command.EffectiveParentId, command.SortOrder);

            if (command.EffectiveParentId.HasValue)
            {
                var tasks = await db.Todo.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
                TodoHierarchyValidator.EnsureCanMove(task, command.EffectiveParentId, tasks.Append(task).ToArray());
            }

            await db.Todo.AddAsync(task, ct);
            return task.UID;
        }
    }

    public sealed class UpdateTodoCommandHandler(IApplicationDbContext db)
        : IRequestHandler<UpdateTodoCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateTodoCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();

            if (command.Status == TodoStatus.Completed && task.Status != TodoStatus.Completed)
            {
                var incomplete = await db.TodoDependencies
                    .Where(x => x.PlanId == task.PlanId && x.SuccessorId == task.UID)
                    .Join(db.Todo, d => d.PredecessorId, t => t.UID, (d, t) => t)
                    .AnyAsync(x => x.Status != TodoStatus.Completed, ct);
                if (incomplete)
                    throw new DomainLegalityException("仍有未完成的前置任务，当前任务不能完成");
            }

            if (!Enum.IsDefined(command.Status))
                throw new DomainLegalityException("无效的任务状态");
            task.UpdateDetails(command.EffectiveTitle, command.Description ?? task.Description, command.Priority);
            task.SetStatus(command.Status);
            return task.UID;
        }
    }

    public sealed class SetTodoParentCommandHandler(IApplicationDbContext db)
        : IRequestHandler<SetTodoParentCommand, Guid>
    {
        public async Task<Guid> Handle(SetTodoParentCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();
            var tasks = await db.Todo.Where(x => x.PlanId == task.PlanId).ToListAsync(ct);
            TodoHierarchyValidator.EnsureCanMove(task, command.ParentId, tasks);
            if (command.ParentId.HasValue) task.SetParent(command.ParentId);
            else task.RemoveParent();
            return task.UID;
        }
    }

    public sealed class SetTodoSortOrderCommandHandler(IApplicationDbContext db)
        : IRequestHandler<SetTodoSortOrderCommand, Guid>
    {
        public async Task<Guid> Handle(SetTodoSortOrderCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();
            task.SetSortOrder(command.SortOrder);
            return task.UID;
        }
    }

    public sealed class ArchiveTodoCommandHandler(IApplicationDbContext db)
        : IRequestHandler<ArchiveTodoCommand, Guid>
    {
        public async Task<Guid> Handle(ArchiveTodoCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();
            var hasChildren = await db.Todo.AnyAsync(x => x.FTID == task.UID && !x.IsArchived, ct);
            var hasDependencies = await db.TodoDependencies.AnyAsync(
                x => x.PredecessorId == task.UID || x.SuccessorId == task.UID, ct);
            if (hasChildren || hasDependencies)
                throw new DomainLegalityException("请先处理子任务和依赖关系，再归档该任务");
            task.Archive();
            return task.UID;
        }
    }

    public sealed class RestoreTodoCommandHandler(IApplicationDbContext db)
        : IRequestHandler<RestoreTodoCommand, Guid>
    {
        public async Task<Guid> Handle(RestoreTodoCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();
            if (task.FTID.HasValue && await db.Todo.AnyAsync(
                x => x.UID == task.FTID.Value && x.IsArchived, ct))
                throw new DomainLegalityException("请先恢复父任务，再恢复当前任务");
            task.Restore();
            return task.UID;
        }
    }

    public sealed class DeleteTodoCommandHandler(IApplicationDbContext db)
        : IRequestHandler<DeleteTodoCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteTodoCommand command, CancellationToken ct)
        {
            var task = await db.Todo.FirstOrDefaultAsync(x => x.UID == command.UID, ct)
                ?? throw new ResourceNotFoundException("Todo 不存在");
            var plan = await db.DevelopmentPlans.FirstAsync(x => x.UID == task.PlanId, ct);
            plan.EnsureActive();
            if (task.Status == TodoStatus.Completed)
                throw new DomainLegalityException("已完成任务不能直接删除，请使用归档");
            if (await db.Todo.AnyAsync(x => x.FTID == task.UID, ct))
                throw new DomainLegalityException("存在子任务，不能删除父任务");
            if (await db.TodoDependencies.AnyAsync(
                x => x.PredecessorId == task.UID || x.SuccessorId == task.UID, ct))
                throw new DomainLegalityException("存在依赖关系，请先移除依赖再删除任务");
            db.Todo.Remove(task);
            return Unit.Value;
        }
    }

    public sealed class AddTodoDependencyCommandHandler(IApplicationDbContext db)
        : IRequestHandler<AddTodoDependencyCommand, Guid>
    {
        public async Task<Guid> Handle(AddTodoDependencyCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.PlanId, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            plan.EnsureActive();
            var tasks = await db.Todo.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
            var dependencies = await db.TodoDependencies.Where(x => x.PlanId == command.PlanId).ToListAsync(ct);
            var edge = new TodoDependencyEntity(command.PlanId, command.PredecessorId, command.SuccessorId);
            TodoDependencyGraphValidator.EnsureCanAdd(edge, tasks, dependencies);
            await db.TodoDependencies.AddAsync(edge, ct);
            plan.IncrementDependencyVersion();
            return edge.UID;
        }
    }

    public sealed class RemoveTodoDependencyCommandHandler(IApplicationDbContext db)
        : IRequestHandler<RemoveTodoDependencyCommand, Guid>
    {
        public async Task<Guid> Handle(RemoveTodoDependencyCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.PlanId, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            plan.EnsureActive();
            var edge = await db.TodoDependencies.FirstOrDefaultAsync(x =>
                x.PlanId == command.PlanId && x.PredecessorId == command.PredecessorId &&
                x.SuccessorId == command.SuccessorId, ct)
                ?? throw new ResourceNotFoundException("任务依赖关系不存在");
            db.TodoDependencies.Remove(edge);
            plan.IncrementDependencyVersion();
            return edge.UID;
        }
    }

    public sealed class SaveRoadmapNodeLayoutCommandHandler(IApplicationDbContext db)
        : IRequestHandler<SaveRoadmapNodeLayoutCommand, Guid>
    {
        public async Task<Guid> Handle(SaveRoadmapNodeLayoutCommand command, CancellationToken ct)
        {
            var plan = await db.DevelopmentPlans.FirstOrDefaultAsync(x => x.UID == command.PlanId, ct)
                ?? throw new ResourceNotFoundException("开发计划不存在");
            plan.EnsureActive();
            if (!await db.Todo.AnyAsync(x => x.UID == command.TodoId && x.PlanId == command.PlanId && !x.IsArchived, ct))
                throw new ResourceNotFoundException("科技树节点不存在或不属于当前计划");

            var layout = await db.RoadmapNodeLayouts.FirstOrDefaultAsync(x => x.TodoId == command.TodoId, ct);
            if (layout is null)
            {
                layout = new RoadmapNodeLayoutEntity(command.PlanId, command.TodoId, command.X, command.Y);
                await db.RoadmapNodeLayouts.AddAsync(layout, ct);
            }
            else
            {
                if (layout.PlanId != command.PlanId)
                    throw new DomainLegalityException("布局与任务所属计划不一致");
                layout.SetPosition(command.X, command.Y);
            }
            return layout.UID;
        }
    }
}