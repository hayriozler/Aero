using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
{
    public void Configure(EntityTypeBuilder<Aircraft> builder)
    {
        builder.ToTable("Aircraft", "metadata");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.AircraftTypeCode).IsRequired().HasMaxLength(5);
        builder.Property(a => a.AirlineCode).HasMaxLength(3);
        builder.Property(a => a.Owner).HasMaxLength(100);
        builder.Property(a => a.SizeCategory).HasMaxLength(1);
        builder.Property(a => a.NoiseCategory).HasMaxLength(10);
        builder.Property(a => a.Weight).HasPrecision(18, 4);

        builder.HasOne<AircraftType>().WithMany().HasForeignKey(a => a.AircraftTypeCode).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Airline>().WithMany().HasForeignKey(a => a.AirlineCode);
        builder.HasOne<AircraftSizeCategory>().WithMany().HasForeignKey(a => a.SizeCategory);
        builder.HasOne<AircraftNoiseCategory>().WithMany().HasForeignKey(a => a.NoiseCategory);

        AuditConfiguration.Apply<Aircraft, string>(builder);
        builder.HasAuditType<Aircraft, AircraftAudit>();
    }
}
