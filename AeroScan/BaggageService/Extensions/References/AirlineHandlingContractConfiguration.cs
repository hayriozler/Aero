using Domain.Aggregates.HandlingContracts;
using Domain.Audits;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BaggageService.Extensions.References;

internal sealed class AirlineHandlingContractConfiguration
    : IEntityTypeConfiguration<AirlineHandlingContract>
{
    public void Configure(EntityTypeBuilder<AirlineHandlingContract> builder)
    {
        builder.ToTable("AirlineHandlingContracts", "references");
        builder.HasKey(c => c.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn().ValueGeneratedOnAdd();

        builder.Property(c => c.AirlineCode)
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(c => c.ValidFrom).IsRequired();
        builder.Property(c => c.ValidTo).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.Notes).HasMaxLength(500);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();


        builder.Property(h => h.CreatedAt).IsRequired();
        builder.Property(h => h.CreatedBy).HasMaxLength(20).IsRequired();
        builder.Property(h => h.UpdatedAt).IsRequired();
        builder.Property(h => h.UpdatedBy).HasMaxLength(20).IsRequired();


        builder.HasOne(c => c.HandlingCompany)
            .WithMany()
            .HasForeignKey(c => c.HandlingCompanyCode)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.FlightNumbers)
            .WithOne()
            .HasForeignKey(fn => fn.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.FlightNumbers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasAuditType<AirlineHandlingContract, AirlineHandlingContractLog>();
        builder.HasIndex(c => new { c.AirlineCode, c.IsActive });
        builder.HasIndex(c => new { c.ValidFrom, c.ValidTo });
    }
}
