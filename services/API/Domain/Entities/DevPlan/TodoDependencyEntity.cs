using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.DevPlan
{
    [Table("TodoDependencies")]
    [Index(nameof(PredecessorId), nameof(SuccessorId), IsUnique = true)]
    [Index(nameof(PlanId), nameof(PredecessorId))]
    [Index(nameof(PlanId), nameof(SuccessorId))]
    public sealed class TodoDependencyEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid PlanId { get; init; } = Guid.Empty;

        public Guid PredecessorId { get; private set; } = Guid.Empty;

        public Guid SuccessorId { get; private set; } = Guid.Empty;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        
        [ForeignKey(nameof(PredecessorId))]
        public TodoEntity PredecessorTodo { get; set; } = null!;

        [ForeignKey(nameof(SuccessorId))]
        public TodoEntity SuccessorTodo { get; set; } = null!;

        public TodoDependencyEntity() { }

        public TodoDependencyEntity(Guid planId, Guid predecessorId, Guid successorId)
        {
            if (planId == Guid.Empty)
                throw new DomainLegalityException("开发计划 ID 不能为 Empty");

            if (predecessorId == Guid.Empty || successorId == Guid.Empty)
                throw new DomainLegalityException("依赖任务 ID 不能为 Empty");

            if (predecessorId == successorId)
                throw new DomainLegalityException("任务不能依赖自身");

            PlanId = planId;
            PredecessorId = predecessorId;
            SuccessorId = successorId;
        }
    }
}
