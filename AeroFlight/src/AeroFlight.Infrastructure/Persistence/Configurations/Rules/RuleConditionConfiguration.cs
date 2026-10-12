using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Rules;

internal sealed class RuleConditionConfiguration : IEntityTypeConfiguration<RuleCondition>
{
    public void Configure(EntityTypeBuilder<RuleCondition> builder)
    {
        builder.ToTable("RuleCondition", "ops");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Field).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Operator).HasConversion<string>().IsRequired().HasMaxLength(20);
        builder.Property(c => c.Value).HasMaxLength(500);

        builder.HasIndex(c => c.GroupId);
    }
}
