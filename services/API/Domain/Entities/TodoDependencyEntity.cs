using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("TodoDependencies")]
    [Index(nameof(PredecessorId), nameof(SuccessorId), IsUnique = true)]
    [Index(nameof(PlanId), nameof(PredecessorId))]
    [Index(nameof(PlanId), nameof(SuccessorId))]
    public class TodoDependencyEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid PlanId { get; private set; }

        public Guid PredecessorId { get; private set; }

        public Guid SuccessorId { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

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
