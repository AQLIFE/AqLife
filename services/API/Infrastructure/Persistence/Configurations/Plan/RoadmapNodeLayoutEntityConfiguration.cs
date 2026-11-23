using AqLife.Domain.Entities.DevPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Plan;

public sealed class RoadmapNodeLayoutEntityConfiguration : IEntityTypeConfiguration<RoadmapNodeLayoutEntity>
{
    public void Configure(EntityTypeBuilder<RoadmapNodeLayoutEntity> builder)
    {
        builder.HasKey(layout => layout.UID);

        builder.HasOne<DevelopmentPlanEntity>()
            .WithMany()
            .HasForeignKey(layout => layout.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}