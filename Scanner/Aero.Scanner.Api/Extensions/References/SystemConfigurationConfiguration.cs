using Aero.Scanner.Domain.Aggregates.Settings;
using Aero.Scanner.Domain.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Scanner.Api.Extensions.References;

internal sealed class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations", "references");
        builder.HasKey(r => r.Key);

        builder.Property(r => r.Key)
            .HasMaxLength(25)
            .HasColumnType("citext")
            .HasColumnName("Key")
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(100);

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasAuditType<SystemConfiguration, SystemConfigurationLog>();
        builder.HasAuditDisplayName(e => e.Key, "Name");
        builder.HasAuditDisplayName(e => e.Value, "Value");
        builder.HasAuditDisplayName(e => e.Description, "Description");

    }
}
