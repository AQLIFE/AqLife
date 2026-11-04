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

        public Guid PlanId { get; private set; } = DevelopmentPlanEntity.DefaultPlanId;

        // Title is the domain property; the existing database column remains Desc.
        [Column("Desc"), Required]
        public string Title { get; private set; } = string.Empty;

        [NotMapped]
        public string Desc => Title;

        public string? Description { get; private set; }

        public Guid? FTID { get; private set; }

        [Required]
        public TodoStatus Status { get; private set; } = TodoStatus.Initial;

        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompletedAt { get; private set; }
        public bool IsArchived { get; private set; }

        [Range(1, 99)]
        public int Priority { get; private set; } = 1;

        [Range(0, int.MaxValue)]
        public int SortOrder { get; private set; }

        public ICollection<TodoEntity> Children { get; set; } = [];

        [NotMapped]
        public bool HasChildren => Children.Count > 0;

        public TodoEntity() { }

        public TodoEntity(string title) : this(DevelopmentPlanEntity.DefaultPlanId, title) { }

        public TodoEntity(string title, int priority) : this(DevelopmentPlanEntity.DefaultPlanId, title, priority: priority) { }

        public TodoEntity(string title, int priority, Guid? ftid)
            : this(DevelopmentPlanEntity.DefaultPlanId, title, priority: priority, parentId: ftid) { }

        public TodoEntity(Guid planId, string title, string? description = null, int priority = 1, Guid? parentId = null, int sortOrder = 0)
        {
            if (planId == Guid.Empty)
                throw new DomainLegalityException("开发计划 ID 不能为 Empty");

            PlanId = planId;
            var normalizedTitle = RequireTitle(title);
            var normalizedDescription = NormalizeDescription(description);
            ValidatePriority(priority);
            Title = normalizedTitle;
            Description = normalizedDescription;
            Priority = priority;
            SetParent(parentId);
            SetSortOrder(sortOrder);
        }

        public void UpdateDetails(string title, string? description, int priority)
        {
            EnsureActive();
            var normalizedTitle = RequireTitle(title);
            var normalizedDescription = NormalizeDescription(description);
            ValidatePriority(priority);
            Title = normalizedTitle;
            Description = normalizedDescription;
            Priority = priority;
            Touch();
        }

        public void Update(string title, TodoStatus status, int priority)
        {
            if (!Enum.IsDefined(status))
                throw new DomainLegalityException("无效的任务状态");
            UpdateDetails(title, Description, priority);
            SetStatus(status);
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

        public void Completed() => Complete();

        public void Reopen(TodoStatus targetStatus = TodoStatus.Wait)
        {
            if (targetStatus == TodoStatus.Completed)
                throw new DomainLegalityException("重新打开任务时不能指定 Completed 状态");
            SetStatus(targetStatus);
        }

        public void SetParent(Guid? parentId)
        {
            EnsureActive();
            if (parentId == Guid.Empty || parentId == UID)
                throw new DomainLegalityException("父任务 ID 无效");
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

        private static string RequireTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainLegalityException("任务标题不能为空");
            title = title.Trim();
            if (title.Length > 500)
                throw new DomainLegalityException("任务标题不能超过 500 个字符");
            return title;
        }

        private static string? NormalizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            description = description.Trim();
            if (description.Length > 10000)
                throw new DomainLegalityException("任务描述不能超过 10000 个字符");
            return description;
        }

        private static void ValidatePriority(int priority)
        {
            if (priority is < 1 or > 99)
                throw new DomainLegalityException("任务优先级必须介于 1 到 99");
        }

        private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}