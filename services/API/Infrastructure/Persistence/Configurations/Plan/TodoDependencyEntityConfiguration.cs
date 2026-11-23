using AqLife.Domain.Entities.DevPlan;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Plan;

public sealed class TodoDependencyEntityConfiguration : IEntityTypeConfiguration<TodoDependencyEntity>
{
    public void Configure(EntityTypeBuilder<TodoDependencyEntity> builder)
    {
        builder.HasOne<DevelopmentPlanEntity>()
            .WithMany()
            .HasForeignKey(dependency => dependency.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dependency => dependency.PredecessorTodo)
            .WithMany(todo => todo.OutgoingDependencies)
            .HasForeignKey(dependency => dependency.PredecessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(dependency => dependency.SuccessorTodo)
            .WithMany(todo => todo.IncomingDependencies)
            .HasForeignKey(dependency => dependency.SuccessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}