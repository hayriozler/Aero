using Aero.Flights.Domain.Audit.Configuration;
using Aero.Flights.Domain.Configuration;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.AeroConfigurations;

internal sealed class AeroConfigurationConfiguration : IEntityTypeConfiguration<AeroConfiguration>
{
    public void Configure(EntityTypeBuilder<AeroConfiguration> builder)
    {
        builder.ToTable("AeroConfiguration", "ops");
        builder.HasKey(a => a.Id);
        AuditConfiguration.Apply<AeroConfiguration, int>(builder);

        builder.Property(a => a.ConfigurationName).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Value).HasMaxLength(500);
        builder.Property(a => a.ConfigType).HasMaxLength(20);
        builder.Property(a => a.Description).HasMaxLength(500);

        builder.HasAuditType<AeroConfiguration, AeroConfigurationAudit>();
    }
}
