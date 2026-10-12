using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class AircraftTypeConfiguration : IEntityTypeConfiguration<AircraftType>
{
    public void Configure(EntityTypeBuilder<AircraftType> builder)
    {
        builder.ToTable("AircraftType", "metadata");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasMaxLength(6).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.IataCode).HasMaxLength(3);
        builder.Property(a => a.IataSubCode).HasMaxLength(3);
        builder.Property(a => a.IcaoCode).HasMaxLength(4);
        builder.Property(a => a.SizeCategory).HasMaxLength(1);
        builder.Property(a => a.NoiseCategory).HasMaxLength(10);
        builder.Property(a => a.Manufacturer).HasMaxLength(100);
        builder.Property(a => a.SeatCapacity);
        builder.Property(a => a.Model).HasMaxLength(100);
        builder.Property(a => a.Mtow).HasPrecision(18, 4);
        builder.Property(a => a.LandingWeight).HasPrecision(18, 4);

        builder.HasOne<AircraftSizeCategory>().WithMany().HasForeignKey(a => a.SizeCategory);
        builder.HasOne<AircraftNoiseCategory>().WithMany().HasForeignKey(a => a.NoiseCategory);

        AuditConfiguration.Apply<AircraftType, string>(builder);
        builder.HasAuditType<AircraftType, AircraftTypeAudit>();

    }
}
