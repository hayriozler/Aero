using AeroFlight.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Rules;

internal sealed class BusinessRuleConfiguration : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> builder)
    {
        builder.ToTable("Rule", "ops");
        builder.HasKey(r => r.Id);
        AuditConfiguration.Apply<Rule, int>(builder);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.IsEnabled).IsRequired();
        builder.Property(r => r.StopOnMatch).IsRequired();

        builder.HasMany(r => r.Triggers).WithOne().HasForeignKey(t => t.RuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(r => r.ConditionGroups).WithOne().HasForeignKey(g => g.RuleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(r => r.Actions).WithOne().HasForeignKey(a => a.RuleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.IsEnabled);
    }
}
