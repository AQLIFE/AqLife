using AqLife.Domain.Entities.DevPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Plan;

public sealed class TodoEntityConfiguration : IEntityTypeConfiguration<TodoEntity>
{
    public void Configure(EntityTypeBuilder<TodoEntity> builder)
    {
        builder.Property(todo => todo.PlanId)
            .HasDefaultValue(DevelopmentPlanEntity.DefaultPlanId);

        builder.HasOne(todo => todo.Plan)
            .WithMany(plan => plan.Todos)
            .HasForeignKey(todo => todo.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(todo => todo.LayoutEntity)
            .WithOne()
            .HasForeignKey<RoadmapNodeLayoutEntity>(layout => layout.UID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}