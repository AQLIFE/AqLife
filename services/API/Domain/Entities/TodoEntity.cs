using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using AqLife.Shared.Options;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("TodoList")]
    [Index(nameof(PlanId), nameof(FTID), nameof(SortOrder))]
    [Index(nameof(PlanId), nameof(Status))]
    public class TodoEntity : IEntity
    {
        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Every task belongs to exactly one plan. Existing creation flows temporarily
        /// use DefaultPlanId until their commands expose explicit plan selection.
        /// </summary>
        public Guid PlanId { get; private set; } = DevelopmentPlanEntity.DefaultPlanId;

        /// <summary>
        /// Legacy persisted column. The domain-facing name is Title.
        /// </summary>
        [Column("Desc")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// Compatibility alias for the existing application/DTO mapping.
        /// EF persists Title to the existing Desc column.
        /// </summary>
        [NotMapped]
        public string Desc => Title;

        public string? Description { get; private set; }

        /// <summary>
        /// Parent task ID. A task has at most one parent; hierarchy depth is unlimited.
        /// </summary>
        public Guid? FTID { get; private set; }

        [Required]
        public TodoStatus Status { get; private set; } = TodoStatus.Initial;

        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? CompletedAt { get; private set; }

        /// <summary>
        /// Archived/deprecated tasks are kept out of active workflows without overloading Priority.
        /// </summary>
        public bool IsArchived { get; private set; }

        /// <summary>
        /// Higher values represent higher priority. Status is never encoded in Priority.
        /// </summary>
        [Range(1, 99)]
        public int Priority { get; private set; } = 1;

        [Range(0, int.MaxValue)]
        public int SortOrder { get; private set; }

        public ICollection<TodoEntity> Children { get; set; } = [];

        [NotMapped]
        public bool HasChildren => Children.Count > 0;

        public TodoEntity() { }

        // Legacy constructors keep existing application code source-compatible and
        // attach new legacy-created tasks to the seeded default plan.
        public TodoEntity(string desc)
        {
            Title = RequireTitle(desc);
        }

        public TodoEntity(string desc, int priority) : this(desc)
        {
            SetPriority(priority);
        }

        public TodoEntity(string desc, int priority, Guid? ftid) : this(desc, priority)
        {
            SetParent(ftid);
        }

        public TodoEntity(
            Guid planId,
            string title,
            string? description = null,
            int priority = 1,
            Guid? parentId = null,
            int sortOrder = 0)
        {
            PlanId = Guid.Empty;
            AssignToPlan(planId);
            Title = RequireTitle(title);
            Description = NormalizeDescription(description);
            SetPriority(priority);
            SetParent(parentId);
            SetSortOrder(sortOrder);
        }

        public void AssignToPlan(Guid planId)
        {
            EnsureActive();
            if (planId == Guid.Empty)
                throw new DomainLegalityException("开发计划 ID 不能为 Empty");

            if (PlanId != Guid.Empty && PlanId != planId)
                throw new DomainLegalityException("任务不能直接变更所属开发计划");

            PlanId = planId;
            Touch();
        }

        public void SetParent(Guid? parentId)
        {
            EnsureActive();
            if (parentId == Guid.Empty)
                throw new DomainLegalityException("父任务 ID 不能为 Empty");

            if (parentId == UID)
                throw new DomainLegalityException("任务不能成为自己的父任务");

            FTID = parentId;
            Touch();
        }

        public void RemoveParent()
        {
            EnsureActive();
            FTID = null;
            Touch();
        }

        public void SetSortOrder(int sortOrder)
        {
            EnsureActive();
            if (sortOrder < 0)
                throw new DomainLegalityException("任务排序值不能小于 0");

            SortOrder = sortOrder;
            Touch();
        }

        public void Update(string content, TodoStatus status, int priority)
            => Update(content, Description, status, priority);

        public void Update(string title, string? description, TodoStatus status, int priority)
        {
            EnsureActive();
            if (Status == TodoStatus.Completed)
                throw new DomainLegalityException("已完成任务需要先重新打开才能修改");

            var normalizedTitle = RequireTitle(title);
            var normalizedDescription = NormalizeDescription(description);
            ValidateStatus(status);
            ValidatePriority(priority);

            Title = normalizedTitle;
            Description = normalizedDescription;
            Priority = priority;

            if (status == TodoStatus.Completed)
                Complete();
            else
            {
                Status = status;
                CompletedAt = null;
                Touch();
            }
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

        // Kept for compatibility with existing callers.
        public void Completed() => Complete();

        public void Reopen(TodoStatus targetStatus = TodoStatus.Wait)
        {
            EnsureActive();
            ValidateStatus(targetStatus);
            if (targetStatus == TodoStatus.Completed)
                throw new DomainLegalityException("重新打开任务时不能指定 Completed 状态");

            Status = targetStatus;
            CompletedAt = null;
            Touch();
        }

        public void Archive()
        {
            if (IsArchived)
                return;

            IsArchived = true;
            Touch();
        }

        public void Restore()
        {
            if (!IsArchived)
                return;

            IsArchived = false;
            Touch();
        }

        private void EnsureActive()
        {
            if (IsArchived)
                throw new DomainLegalityException("已归档任务不允许修改");
        }

        private void SetPriority(int priority)
        {
            ValidatePriority(priority);
            Priority = priority;
        }

        private static void ValidatePriority(int priority)
        {
            if (priority is < 1 or > 99)
                throw new DomainLegalityException("任务优先级必须介于 1 到 99");
        }

        private static void ValidateStatus(TodoStatus status)
        {
            if (!Enum.IsDefined(status))
                throw new DomainLegalityException("无效的任务状态");
        }

        private static string RequireTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainLegalityException("任务标题不能为空");

            return title.Trim();
        }

        private static string? NormalizeDescription(string? description)
            => string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}
