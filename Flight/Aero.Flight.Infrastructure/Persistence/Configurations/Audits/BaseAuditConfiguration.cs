using Aero.Flights.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Audits;

internal abstract class BaseAuditConfiguration<T> : IEntityTypeConfiguration<T>
    where T : AuditLogBase
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(typeof(T).Name, "audits");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(a => a.PrimaryKey).HasColumnType("jsonb").IsRequired();
        builder.Property(a => a.Action).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Snapshot).HasColumnType("jsonb").IsRequired();
        builder.Property(a => a.Timestamp).IsRequired();
        builder.Property(a => a.ChangedBy).HasMaxLength(100).IsRequired();

        builder.HasIndex(a => new { a.Action, a.Timestamp });
    }
}
