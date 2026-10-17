using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Domain.Audit.MetaData;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class HandlingServiceConfiguration : IEntityTypeConfiguration<HandlingService>
{
    public void Configure(EntityTypeBuilder<HandlingService> builder)
    {
        builder.ToTable("HandlingService", "metadata");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).IsRequired().HasMaxLength(5).HasColumnType("citext").HasColumnName("Code");
        builder.Property(t => t.Unit).HasMaxLength(15);
        builder.Property(t => t.Description).HasMaxLength(200);

        AuditConfiguration.Apply<HandlingService, string>(builder);
        builder.HasAuditType<HandlingService, HandlingServiceAudit>();

    }
}
