using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class GateConfiguration : IEntityTypeConfiguration<Gate>
{
    public void Configure(EntityTypeBuilder<Gate> builder)
    {
        builder.ToTable("Gate", "metadata");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).IsRequired().HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(g => g.GateTypeCode).HasMaxLength(1);
        builder.Property(g => g.TerminalCode).HasMaxLength(10);
        builder.Property(g => g.Description).HasMaxLength(200);

        builder.HasOne<GateType>().WithMany().HasForeignKey(g => g.GateTypeCode);
        builder.HasOne<Terminal>().WithMany().HasForeignKey(g => g.TerminalCode);

        AuditConfiguration.Apply<Gate, string>(builder);
        builder.HasAuditType<Gate, GateAudit>();

    }
}
