using AqLife.Domain.Entities.DevPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Plan;

public sealed class DevelopmentPlanEntityConfiguration : IEntityTypeConfiguration<DevelopmentPlanEntity>
{
    public void Configure(EntityTypeBuilder<DevelopmentPlanEntity> builder)
    {
        builder.HasData(new
        {
            UID = DevelopmentPlanEntity.DefaultPlanId,
            Name = "默认开发计划",
            Description = (string?)null,
            IsArchived = false,
            CreatedAt = DateTimeOffset.UnixEpoch,
            UpdatedAt = DateTimeOffset.UnixEpoch,
            DependencyVersion = 0L
        });

        builder.Property(plan => plan.DependencyVersion)
            .IsConcurrencyToken();
    }
}