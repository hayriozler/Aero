using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Rules;

internal sealed class RuleActionConfiguration : IEntityTypeConfiguration<RuleAction>
{
    public void Configure(EntityTypeBuilder<RuleAction> builder)
    {
        builder.ToTable("RuleAction", "ops");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActionType).HasConversion<string>().IsRequired().HasMaxLength(20);
        builder.Property(a => a.TargetField).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Value).HasMaxLength(500);
        builder.Property(a => a.MatchValue).HasMaxLength(100);
        builder.Property(a => a.Order).IsRequired();

        builder.HasIndex(a => a.RuleId);
    }
}
