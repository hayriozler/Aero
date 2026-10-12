using Domain.Aggregates.Flights;
using Domain.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Extensions;
namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class DepartureFlightContainerConfiguration : IEntityTypeConfiguration<Container>
{
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        builder.ToTable("DepartureFlightContainers", "flights");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityColumn();
        builder.Property(p => p.FlightId).IsRequired();
        builder.Property(p => p.ContainerCode).IsRequired();
        builder.Property(p => p.ContainerTypeCode).IsRequired();
        builder.Property(p => p.ContainerStatusCode).IsRequired();
        builder.Property(p => p.ContainerClassCode).IsRequired();
        builder.Property(p => p.ContainerDestination).IsRequired();

        builder.HasIndex(p => p.FlightId);

        // Container'ın kendi audit tablosu
        builder.HasAuditType<Container, ContainerLog>();

        // Container değiştiğinde DepartureFlightLog'a da "ContainerAdded/Modified/Removed" yaz
        builder.HasCrossAudit<Container, DepartureFlightLog>(c => c.FlightId, "Container");

        builder.HasAuditDisplayName(c => c.ContainerCode, "Container Code");
        builder.HasAuditDisplayName(c => c.ContainerTypeCode, "Type");
        builder.HasAuditDisplayName(c => c.ContainerStatusCode, "Status");
        builder.HasAuditDisplayName(c => c.ContainerClassCode, "Class");
        builder.HasAuditDisplayName(c => c.ContainerDestination, "Destination");
    }
}


