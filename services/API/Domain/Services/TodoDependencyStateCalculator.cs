using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;

namespace AqLife.Domain.Services
{
    public sealed record TodoDependencyState(
        bool IsBlocked,
        int IncompleteDependencyCount,
        IReadOnlyCollection<Guid> BlockingTodoIds);

    /// <summary>
    /// Calculates the derived Blocked state. Blocked is never persisted and never
    /// overrides a task's Completed status.
    /// </summary>
    public static class TodoDependencyStateCalculator
    {
        public static TodoDependencyState Calculate(
            TodoEntity task,
            IEnumerable<TodoDependencyEntity> dependencies,
            IReadOnlyDictionary<Guid, TodoEntity> tasksById)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(dependencies);
            ArgumentNullException.ThrowIfNull(tasksById);

            if (task.Status == TodoStatus.Completed)
                return new TodoDependencyState(false, 0, Array.Empty<Guid>());

            var incoming = dependencies
                .Where(x => x.SuccessorId == task.UID)
                .ToArray();

            var blockingIds = new List<Guid>();
            foreach (var dependency in incoming)
            {
                if (dependency.PlanId != task.PlanId)
                    throw new DomainLegalityException("任务依赖必须属于同一个开发计划");

                if (!tasksById.TryGetValue(dependency.PredecessorId, out var predecessor))
                    throw new DomainLegalityException("任务依赖引用了不存在的前置任务");

                if (predecessor.PlanId != task.PlanId)
                    throw new DomainLegalityException("任务依赖不能跨开发计划");

                if (predecessor.Status != TodoStatus.Completed)
                    blockingIds.Add(predecessor.UID);
            }

            return new TodoDependencyState(
                blockingIds.Count > 0,
                blockingIds.Count,
                blockingIds);
        }
    }
}
