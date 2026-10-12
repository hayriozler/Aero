using Domain.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Audit;

internal class BaseAuditConfiguration<T> : IEntityTypeConfiguration<T>
    where T: AuditLogBase
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(typeof(T).Name, "audits");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(a => a.PrimaryKey)
            .HasColumnType("jsonb")      
            .IsRequired();

        builder.Property(a => a.Action)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(a => a.Snapshot)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(a => a.Timestamp).IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(a => new { a.Action, a.Timestamp });
    }
}
