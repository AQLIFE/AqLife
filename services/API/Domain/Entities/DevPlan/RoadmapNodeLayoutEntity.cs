using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.DevPlan
{
    [Table("RoadmapNodeLayouts")]
    [Index(nameof(PlanId))]
    public class RoadmapNodeLayoutEntity : IEntity
    {
        [Key]// 同时作为外键,引用自 Todo 表
        public Guid UID { get; init; } 
        //public Guid TodoId { get; init; } = Guid.Empty;// 该模型的真正主键

        public Guid PlanId { get; init; } = Guid.Empty;


        public double X { get; private set; } = 0.0;

        public double Y { get; private set; } = 0.0;

        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public RoadmapNodeLayoutEntity() { }

        public RoadmapNodeLayoutEntity(Guid todoId, Guid planId, double x, double y)
        {
            if (planId == Guid.Empty || todoId == Guid.Empty)
                throw new DomainLegalityException("计划 ID 和任务 ID 不能为 Empty");
            if (planId == todoId)
                throw new DomainLegalityException("计划ID与 任务Id 相同");//增加容错,避免后续开发过程中制造低级错误

            PlanId = planId;
            UID = todoId;
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
