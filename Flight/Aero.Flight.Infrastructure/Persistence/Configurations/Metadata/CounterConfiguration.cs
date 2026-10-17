using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CounterConfiguration : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        builder.ToTable("Counter", "metadata");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().HasMaxLength(5).HasColumnType("citext").HasColumnName("Code");

        builder.Property(c => c.TerminalCode).HasMaxLength(10);
        builder.Property(c => c.Description).HasMaxLength(200);

        builder.HasOne<Terminal>().WithMany().HasForeignKey(g => g.TerminalCode);

        AuditConfiguration.Apply<Counter, string>(builder);
        builder.HasAuditType<Counter, CounterAudit>();

    }
}
