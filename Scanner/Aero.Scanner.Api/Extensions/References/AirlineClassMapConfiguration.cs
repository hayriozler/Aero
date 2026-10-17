using Aero.Scanner.Domain.Aggregates.Mappings;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

internal sealed class AirlineClassMapConfiguration : IEntityTypeConfiguration<AirlineClassMap>
{
    public void Configure(EntityTypeBuilder<AirlineClassMap> builder)
    {
        builder.ToTable("AirlineClassMap", "references");

        builder.HasKey(p => new { p.AirlineCode, p.SourceClass });

        builder.Property(u => u.AirlineCode)
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(u => u.SourceClass)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(u => u.TargetClass)
            .HasMaxLength(1)
            .IsRequired();


        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasIndex(p => new { p.AirlineCode, p.SourceClass }).IncludeProperties(p => p.TargetClass);

        builder.HasAuditType<AirlineClassMap, AirlineClassMapLog>();
        builder.HasAuditDisplayName(e => e.AirlineCode, "Airline Code");
        builder.HasAuditDisplayName(e => e.SourceClass, "Source Class");
        builder.HasAuditDisplayName(e => e.TargetClass, "Target Class");
    }
}

