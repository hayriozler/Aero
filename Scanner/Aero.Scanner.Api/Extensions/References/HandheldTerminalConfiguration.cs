using Aero.Scanner.Domain.Aggregates.HHTs;
using Aero.Scanner.Domain.Audits;
using Aero.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Aero.Scanner.Api.Extensions.References;

public sealed class HandheldTerminalConfiguration : IEntityTypeConfiguration<HandheldTerminal>
{
    public void Configure(EntityTypeBuilder<HandheldTerminal> builder)
    {
        builder.ToTable("Hhts", "references");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(h => h.DeviceId)
            .HasColumnType("citext")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.Name)
            .HasMaxLength(100)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(h => h.SerialNumber)
            .HasColumnType("citext")
            .HasMaxLength(100);

        builder.Property(h => h.Model)
            .HasMaxLength(100);

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

        builder.HasIndex(h => h.DeviceId).IsUnique();
        builder.HasIndex(h => h.AssignedCompanyCode);

        builder.HasAuditType<HandheldTerminal, HandheldTerminalLog>();
        builder.HasAuditDisplayName(e => e.DeviceId, "Device ID");
        builder.HasAuditDisplayName(e => e.SerialNumber, "Serial Number");
        builder.HasAuditDisplayName(e => e.AssignedCompanyCode, "Assigned Company");

        builder.HasOne(h => h.AssignedCompany)
            .WithMany()
            .HasForeignKey(h => h.AssignedCompanyCode)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();

    }
}
