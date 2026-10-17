using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("Terminal", "metadata");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).IsRequired().HasMaxLength(5).HasColumnType("citext").HasColumnName("Code");
        builder.Property(t => t.Description).HasMaxLength(200);

        AuditConfiguration.Apply<Terminal, string>(builder);
        builder.HasAuditType<Terminal, TerminalAudit>();

    }
}
