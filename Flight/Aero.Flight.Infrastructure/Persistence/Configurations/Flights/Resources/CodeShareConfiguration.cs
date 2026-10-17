using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Ops.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Flights.Resources;

internal sealed class CodeShareConfiguration : IEntityTypeConfiguration<OpsCodeShare>
{
    public void Configure(EntityTypeBuilder<OpsCodeShare> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FlightId).IsRequired();
        builder.Property(c => c.AirlineCode).IsRequired().HasMaxLength(3);
        builder.Property(c => c.FlightNumber).IsRequired().HasMaxLength(10);

        builder.HasOne<Airline>().WithMany().HasForeignKey(c => c.AirlineCode).OnDelete(DeleteBehavior.Restrict);
    }
}
