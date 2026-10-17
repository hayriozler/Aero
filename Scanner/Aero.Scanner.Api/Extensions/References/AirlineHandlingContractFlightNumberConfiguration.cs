using Aero.Scanner.Domain.Aggregates.HandlingContracts;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class AirlineHandlingContractFlightNumberConfiguration
    : IEntityTypeConfiguration<AirlineHandlingContractFlightNumber>
{
    public void Configure(EntityTypeBuilder<AirlineHandlingContractFlightNumber> builder)
    {
        builder.ToTable("AirlineHandlingContractFlightNumbers", "flights");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();
        builder.Property(p => p.FlightNumber)
            .HasMaxLength(7)
            .IsRequired();

        builder.HasAuditType<AirlineHandlingContractFlightNumber, AirlineHandlingContractFlightNumberLog>();
        builder.HasIndex(p => new { p.ContractId, p.FlightNumber }).IsUnique();
    }
}
