using AqLife.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AqLife.Infrastructure.Persistence.Configurations.Account;

public sealed class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasMany(account => account.Subscriptions)
            .WithOne()
            .HasForeignKey(subscription => subscription.AID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}