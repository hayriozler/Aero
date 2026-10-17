using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class RuleActionTypeMetadataConfiguration : IEntityTypeConfiguration<RuleActionTypeMetadata>
{
    public void Configure(EntityTypeBuilder<RuleActionTypeMetadata> builder)
    {
        builder.ToTable("RuleActionType", "metadata");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(30);
        builder.Property(x => x.Description).HasMaxLength(200);
    }
}

internal sealed class RuleTriggerMetadataConfiguration : IEntityTypeConfiguration<RuleTriggerMetadata>
{
    public void Configure(EntityTypeBuilder<RuleTriggerMetadata> builder)
    {
        // Named RuleTriggerOption, not RuleTrigger, so its PK constraint name doesn't collide with
        // ops."RuleTrigger" (RuleTriggerAssignment) — EF's default constraint naming isn't
        // schema-qualified, so two same-named tables in different schemas would still clash.
        builder.ToTable("RuleTriggerOption", "metadata");
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Code).HasMaxLength(30);
        builder.Property(x => x.Description).HasMaxLength(200);
    }
}

internal sealed class RuleFieldMetadataConfiguration : IEntityTypeConfiguration<RuleFieldMetadata>
{
    public void Configure(EntityTypeBuilder<RuleFieldMetadata> builder)
    {
        builder.ToTable("RuleField", "metadata");
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(100);
        builder.Property(x => x.Label).IsRequired().HasMaxLength(200);
        builder.Property(x => x.DataType).IsRequired().HasMaxLength(20);
        builder.Property(x => x.MatchValueLabel).HasMaxLength(100);
    }
}

internal sealed class RuleFieldTriggerMetadataConfiguration : IEntityTypeConfiguration<RuleFieldTriggerMetadata>
{
    public void Configure(EntityTypeBuilder<RuleFieldTriggerMetadata> builder)
    {
        builder.ToTable("RuleFieldTrigger", "metadata");
        builder.HasKey(x => new { x.RuleFieldKey, x.Trigger });
        builder.Property(x => x.Trigger).HasMaxLength(30);
    }
}

internal sealed class RuleFieldActionTypeMetadataConfiguration : IEntityTypeConfiguration<RuleFieldActionTypeMetadata>
{
    public void Configure(EntityTypeBuilder<RuleFieldActionTypeMetadata> builder)
    {
        builder.ToTable("RuleFieldActionType", "metadata");
        builder.HasKey(x => new { x.RuleFieldKey, x.ActionTypeCode });
        builder.Property(x => x.ActionTypeCode).HasMaxLength(30);
    }
}
