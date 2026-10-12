using AeroFlight.Domain.Metadatas;
using AeroFlight.Domain.Audit.MetaData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

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
