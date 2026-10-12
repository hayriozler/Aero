using AeroFlight.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.AeroConfigurations;

internal sealed class SystemPropertyConfiguration : IEntityTypeConfiguration<SystemProperty>
{
    public void Configure(EntityTypeBuilder<SystemProperty> builder)
    {
        builder.ToTable("SystemProperty", "ops");
        builder.HasKey(s => s.Id);
        AuditConfiguration.Apply<SystemProperty, string>(builder);

        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Value).IsRequired().HasMaxLength(500);

        builder.HasIndex(s => s.Code).IsUnique();
    }
}
