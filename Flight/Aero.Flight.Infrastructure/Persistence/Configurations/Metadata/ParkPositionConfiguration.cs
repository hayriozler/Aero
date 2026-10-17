using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class ParkPositionConfiguration : IEntityTypeConfiguration<ParkPosition>
{
    public void Configure(EntityTypeBuilder<ParkPosition> builder)
    {
        builder.ToTable("ParkPosition", "metadata");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(s => s.TypeCode).HasMaxLength(1);
        builder.Property(s => s.Description).HasMaxLength(200);
        builder.Property(s => s.Latitude).HasPrecision(10, 7);
        builder.Property(s => s.Longitude).HasPrecision(10, 7);
        builder.Property(s => s.Angle).HasPrecision(8, 4);

        builder.HasOne<ParkPositionType>().WithMany().HasForeignKey(s => s.TypeCode);

        AuditConfiguration.Apply<ParkPosition, string>(builder);
        builder.HasAuditType<ParkPosition, ParkPositionAudit>();

    }
}
