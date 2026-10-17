using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class RunwayConfiguration : IEntityTypeConfiguration<Runway>
{
    public void Configure(EntityTypeBuilder<Runway> builder)
    {
        builder.ToTable("Runway", "metadata");
        builder.HasKey(r => r.Id);        
        builder.Property(r => r.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(r => r.Name).HasMaxLength(100);
        builder.Property(r => r.Length).HasPrecision(10, 2);
        builder.Property(r => r.Width).HasPrecision(10, 2);

        AuditConfiguration.Apply<Runway, string>(builder);
        builder.HasAuditType<Runway, RunwayAudit>();

    }
}
