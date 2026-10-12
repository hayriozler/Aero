using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AircraftSizeCategoryConfiguration : IEntityTypeConfiguration<AircraftSizeCategory>
{
    public void Configure(EntityTypeBuilder<AircraftSizeCategory> builder)
    {
        builder.ToTable("AircraftSizeCategory", "metadata");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).IsRequired().HasMaxLength(1).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.Description).IsRequired().HasMaxLength(200);

        AuditConfiguration.Apply<AircraftSizeCategory, string>(builder);
        builder.HasAuditType<AircraftSizeCategory, AircraftSizeCategoryAudit>();
    }
}
