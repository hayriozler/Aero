using Aero.Scanner.Domain.Aggregates.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Scanner.Api.Extensions.References;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies", "references");
        builder.HasKey(c => c.Code);

        builder.Property(c => c.Code)
        .HasMaxLength(20)
        .HasColumnType("citext")
        .HasColumnName("Code")
        .IsRequired();


        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .HasColumnType("citext")
            .IsRequired();


        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasAuditType<Company, CompanyLog>();
        builder.HasAuditDisplayName(p => p.Code, "Code");
        builder.HasAuditDisplayName(p => p.Name, "Name");
        builder.HasAuditDisplayName(p => p.Type, "Type");
    }
}

