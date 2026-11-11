using AqLife.Domain.Entities.DevPlan;
using AqLife.Shared.Exceptions;

namespace AqLife.Domain.Services
{
    /// <summary>
    /// Validates hierarchy changes against the task set loaded for one plan.
    /// Persistence/existence checks remain the responsibility of the application layer.
    /// </summary>
    public static class TodoHierarchyValidator
    {
        public static void EnsureCanMove(
            TodoEntity task,
            Guid? newParentId,
            IReadOnlyCollection<TodoEntity> planTasks)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(planTasks);

            if (task.IsArchived)
                throw new DomainLegalityException("已归档任务不能调整层级");

            if (newParentId is null)
                return;

            if (newParentId == Guid.Empty)
                throw new DomainLegalityException("父任务 ID 不能为 Empty");

            if (newParentId == task.UID)
                throw new DomainLegalityException("任务不能成为自己的父任务");

            if (task.PlanId == Guid.Empty)
                throw new DomainLegalityException("任务必须先归属开发计划");

            var parent = planTasks.FirstOrDefault(x => x.UID == newParentId.Value);
            if (parent is null)
                throw new DomainLegalityException("父任务不存在于当前开发计划");

            if (parent.PlanId != task.PlanId)
                throw new DomainLegalityException("父任务与子任务必须属于同一个开发计划");
            if (parent.IsArchived)
                throw new DomainLegalityException("不能将任务移动到已归档的父任务下");

            // Walk upwards from the proposed parent. If the moving task is encountered,
            // the move would make one of its descendants become its ancestor.
            var visited = new HashSet<Guid>();
            TodoEntity? current = parent;

            while (current is not null)
            {
                if (!visited.Add(current.UID))
                    throw new DomainLegalityException("当前任务层级已经存在循环");
                if (current.PlanId != task.PlanId)
                    throw new DomainLegalityException("任务祖先必须属于同一个开发计划");

                if (current.UID == task.UID)
                    throw new DomainLegalityException("不能将任务移动到自己的后代节点下");
                if (current.IsArchived)
                    throw new DomainLegalityException("任务祖先链中存在已归档节点");

            }
        }
    }
}
