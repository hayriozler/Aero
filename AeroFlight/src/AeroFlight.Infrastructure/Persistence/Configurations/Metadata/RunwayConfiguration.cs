using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Audit.MetaData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

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
