using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;

namespace AqLife.Domain.Services
{
    /// <summary>
    /// Validates an AND-semantics dependency graph. Every edge is predecessor -> successor.
    /// The caller supplies all tasks and edges from the same plan snapshot.
    /// </summary>
    public static class TodoDependencyGraphValidator
    {
        public static void EnsureCanAdd(
            TodoDependencyEntity candidate,
            IReadOnlyCollection<TodoEntity> planTasks,
            IEnumerable<TodoDependencyEntity> existingDependencies)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            ArgumentNullException.ThrowIfNull(planTasks);
            ArgumentNullException.ThrowIfNull(existingDependencies);

            if (planTasks.Any(x => x.PlanId != candidate.PlanId))
                throw new DomainLegalityException("任务集合包含其他开发计划的任务");

            var predecessorExists = planTasks.Any(x => x.UID == candidate.PredecessorId);
            var successorExists = planTasks.Any(x => x.UID == candidate.SuccessorId);
            if (!predecessorExists || !successorExists)
                throw new DomainLegalityException("依赖任务必须存在于当前开发计划");

            var predecessor = planTasks.First(x => x.UID == candidate.PredecessorId);
            var successor = planTasks.First(x => x.UID == candidate.SuccessorId);
            if (predecessor.IsArchived || successor.IsArchived)
                throw new DomainLegalityException("归档任务不能建立依赖关系");

            var edges = existingDependencies.ToArray();

            if (edges.Any(x => x.PlanId != candidate.PlanId))
                throw new DomainLegalityException("依赖集合包含其他开发计划的关系");

            if (edges.Any(x =>
                x.PredecessorId == candidate.PredecessorId &&
                x.SuccessorId == candidate.SuccessorId))
                throw new DomainLegalityException("该任务依赖关系已经存在");

            // Adding predecessor -> successor creates a cycle exactly when predecessor
            // is already reachable from successor in the existing graph.
            var adjacency = edges
                .GroupBy(x => x.PredecessorId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.SuccessorId).ToArray());

            var pending = new Stack<Guid>();
            var visited = new HashSet<Guid>();
            pending.Push(candidate.SuccessorId);

            while (pending.Count > 0)
            {
                var current = pending.Pop();

                if (!visited.Add(current))
                    continue;

                if (current == candidate.PredecessorId)
                    throw new DomainLegalityException("添加该依赖关系会形成循环");

                if (adjacency.TryGetValue(current, out var successors))
                {
                    foreach (var sr in successors)
                        pending.Push(sr);
                }
            }
        }
    }
}
