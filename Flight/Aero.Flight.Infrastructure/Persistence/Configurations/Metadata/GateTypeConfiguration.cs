using Aero.Flights.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class GateTypeConfiguration : IEntityTypeConfiguration<GateType>
{
    public void Configure(EntityTypeBuilder<GateType> builder)
    {
        builder.ToTable("GateType", "metadata");
        builder.HasKey(g => g.Code);
        builder.Property(g => g.Code).IsRequired().HasMaxLength(1).HasColumnType("citext");
        builder.Property(g => g.Description).IsRequired().HasMaxLength(200);
    }
}
