using Aero.Flights.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Rules;

internal sealed class RuleTriggerAssignmentConfiguration : IEntityTypeConfiguration<RuleTriggerAssignment>
{
    public void Configure(EntityTypeBuilder<RuleTriggerAssignment> builder)
    {
        builder.ToTable("RuleTrigger", "ops");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Trigger).HasConversion<string>().IsRequired().HasMaxLength(30);

        builder.HasIndex(t => new { t.RuleId, t.Trigger }).IsUnique();
    }
}
