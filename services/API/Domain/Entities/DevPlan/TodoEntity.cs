using AqLife.Shared.Contracts;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities.DevPlan
{
    [Table("TodoList")]
    public class TodoEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        public Guid PlanId { get; init; } = DevelopmentPlanEntity.DefaultPlanId;

        [Required(ErrorMessage = "任务标题不能为空"), StringLength(500, ErrorMessage = "任务标题不能超过 500 个字符")]
        public string Title { get; private set; } = string.Empty;
        [StringLength(10000, ErrorMessage = "任务描述长度不能超过 10000 个字符")]
        public string? Description { get; private set; } = null;

        [Required(ErrorMessage = "任务状态不能为空")]
        public TodoStatus Status { get; private set; } = TodoStatus.Initial;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        [ConcurrencyCheck]
        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; private set; } = null;
        public bool IsArchived { get; private set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "任务排序必须是非负整数")]
        public int SortOrder { get; private set; } = 0;
        
        public DevelopmentPlanEntity Plan { get; set; } = null!;
        public RoadmapNodeLayoutEntity LayoutEntity { get; set; } = null!;
        public ICollection<TodoDependencyEntity> IncomingDependencies { get; set; } = [];
        public ICollection<TodoDependencyEntity> OutgoingDependencies { get; set; } = [];

        public TodoEntity() { }

        public TodoEntity(string title) : this(DevelopmentPlanEntity.DefaultPlanId, title) { }

        public TodoEntity(string title, int sortOrder)
            : this(DevelopmentPlanEntity.DefaultPlanId, title, description: null, sortOrder: sortOrder) { }

        public TodoEntity(Guid planId, string title, string? description = null, int sortOrder = 0)
        {
            if (planId == Guid.Empty)
                throw new DomainLegalityException("开发计划 ID 不能为 Empty");

            this.PlanId = planId;
            this.Title = title.Trim();
            this.Description = description?.Trim();
            this.SortOrder = sortOrder;
            this.Touch();
        }

        public void UpdateDetails(string title, string? description, int sortOrder)
        {
            EnsureActive();
            Title = title;
            Description = description;
            SortOrder = sortOrder;
            Touch();
        }

        public void SetStatus(TodoStatus status)
        {
            EnsureActive();
            if (!Enum.IsDefined(status))
                throw new DomainLegalityException("无效的任务状态");

            if (status == TodoStatus.Completed)
            {
                Complete();
                return;
            }

            Status = status;
            CompletedAt = null;
            Touch();
        }

        public void Complete()
        {
            EnsureActive();
            if (Status == TodoStatus.Completed)
                return;

            Status = TodoStatus.Completed;
            CompletedAt = DateTimeOffset.UtcNow;
            Touch();
        }

        public void Reopen(TodoStatus targetStatus = TodoStatus.Wait)
        {
            if (targetStatus == TodoStatus.Completed)
                throw new DomainLegalityException("重新打开任务时不能指定 Completed 状态");
            SetStatus(targetStatus);
        }
        public void SetSortOrder(int sortOrder)
        {
            EnsureActive();
            SortOrder = sortOrder;
            Touch();
        }

        public void Archive()
        {
            if (IsArchived) return;
            IsArchived = true;
            Touch();
        }

        public void Restore()
        {
            if (!IsArchived) return;
            IsArchived = false;
            Touch();
        }

        private void EnsureActive()
        {
            if (IsArchived)
                throw new DomainLegalityException("已归档任务不允许修改");
        }
        private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}