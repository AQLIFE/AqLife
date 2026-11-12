using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.DevPlan
{
    [Table("RoadmapNodeLayouts")]
    [Index(nameof(TodoId), IsUnique = true)]
    [Index(nameof(PlanId))]
    public class RoadmapNodeLayoutEntity : IEntity
    {
        [NotMapped]// 仅满足 继承链要求
        public Guid UID { get; init; } = Guid.NewGuid();
        public Guid TodoId { get; init; } = Guid.Empty;// 该模型的真正主键

        public Guid PlanId { get; init; } = Guid.Empty;


        public double X { get; private set; } = 0.0;

        public double Y { get; private set; } = 0.0;

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
