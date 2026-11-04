using AqLife.Domain.Entities;
using AqLife.Shared.Exceptions;

namespace AqLife.Domain.Services
{
    /// <summary>
    /// Validates an AND-semantics dependency graph. Every edge is predecessor -> successor.
    /// </summary>
    public static class TodoDependencyGraphValidator
    {
        public static void EnsureCanAdd(
            TodoDependencyEntity candidate,
            IEnumerable<TodoDependencyEntity> existingDependencies)
        {
            ArgumentNullException.ThrowIfNull(candidate);
            ArgumentNullException.ThrowIfNull(existingDependencies);

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
                    foreach (var successor in successors)
                        pending.Push(successor);
                }
            }
        }
    }
}
