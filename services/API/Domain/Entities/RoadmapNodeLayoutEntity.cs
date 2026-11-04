using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("RoadmapNodeLayouts")]
    [Index(nameof(TodoId), IsUnique = true)]
    [Index(nameof(PlanId))]
    public class RoadmapNodeLayoutEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid PlanId { get; private set; }

        public Guid TodoId { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public RoadmapNodeLayoutEntity() { }

        public RoadmapNodeLayoutEntity(Guid planId, Guid todoId, double x, double y)
        {
            if (planId == Guid.Empty || todoId == Guid.Empty)
                throw new DomainLegalityException("计划 ID 和任务 ID 不能为 Empty");

            PlanId = planId;
            TodoId = todoId;
            SetPosition(x, y);
        }

        public void SetPosition(double x, double y)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y))
                throw new DomainLegalityException("科技树节点坐标必须是有限数值");

            X = x;
            Y = y;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
