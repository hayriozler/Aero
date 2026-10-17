using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Rules;

internal sealed class RuleConditionGroupConfiguration : IEntityTypeConfiguration<RuleConditionGroup>
{
    public void Configure(EntityTypeBuilder<RuleConditionGroup> builder)
    {
        builder.ToTable("RuleConditionGroup", "ops");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Order).IsRequired();
        builder.Property(g => g.Operator).HasConversion<string>().IsRequired().HasMaxLength(10);

        builder.HasMany(g => g.Conditions).WithOne().HasForeignKey(c => c.GroupId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => g.RuleId);
    }
}
