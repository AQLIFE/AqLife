using AqLife.Domain.Contracts;
using AqLife.Shared.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AqLife.Domain.Entities
{
    [Table("DevelopmentPlans")]
    public class DevelopmentPlanEntity : IEntity
    {
        /// <summary>
        /// Stable owner for tasks migrated from the legacy single-plan Todo model.
        /// New development plans must use generated IDs.
        /// </summary>
        public static readonly Guid DefaultPlanId = new("00000000-0000-0000-0000-000000000001");

        [Key]
        public Guid UID { get; init; } = Guid.NewGuid();

        [Required, StringLength(128)]
        public string Name { get; private set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; private set; }

        public bool IsArchived { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Monotonically increasing version for dependency-graph mutations.
        /// It is a concurrency aid, not a substitute for a transaction/lock.
        /// </summary>
        public long DependencyVersion { get; private set; }

        public DevelopmentPlanEntity() { }

        public DevelopmentPlanEntity(string name, string? description = null)
        {
            Name = RequireName(name);
            Description = NormalizeDescription(description);
        }

        /// <summary>
        /// Constructor used for deterministic infrastructure seeding.
        /// </summary>
        public DevelopmentPlanEntity(Guid uid, string name, string? description = null)
        {
            if (uid == Guid.Empty)
                throw new DomainLegalityException("开发计划 ID 不能为 Empty");

            UID = uid;
            Name = RequireName(name);
            Description = NormalizeDescription(description);
        }

        public void Rename(string name)
        {
            EnsureActive();
            Name = RequireName(name);
            Touch();
        }

        public void UpdateDescription(string? description)
        {
            EnsureActive();
            Description = NormalizeDescription(description);
            Touch();
        }

        public void Update(string name, string? description)
        {
            EnsureActive();
            var normalizedName = RequireName(name);
            var normalizedDescription = NormalizeDescription(description);
            Name = normalizedName;
            Description = normalizedDescription;
            Touch();
        }

        public void Archive()
        {
            if (UID == DefaultPlanId)
                throw new DomainLegalityException("默认开发计划不能归档");

            if (IsArchived)
                return;

            IsArchived = true;
            Touch();
        }

        public void IncrementDependencyVersion()
        {
            EnsureActive();
            checked { DependencyVersion++; }
            Touch();
        }

        public void EnsureActive()
        {
            if (IsArchived)
                throw new DomainLegalityException("已归档的开发计划不允许修改");
        }

        private static string RequireName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainLegalityException("开发计划名称不能为空");

            name = name.Trim();
            if (name.Length > 128)
                throw new DomainLegalityException("开发计划名称不能超过 128 个字符");

            return name;
        }

        private static string? NormalizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;

            description = description.Trim();
            if (description.Length > 2000)
                throw new DomainLegalityException("开发计划描述不能超过 2000 个字符");

            return description;
        }

        private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}
