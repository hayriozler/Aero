using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company", "metadata");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasMaxLength(10).HasColumnType("citext").HasColumnName("Code");
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.DisplayName).HasMaxLength(100);
        builder.Property(a => a.CompanyTypeCode).HasMaxLength(20).HasColumnType("citext");
        builder.Property(a => a.TaxNumber).HasMaxLength(30);
        builder.Property(a => a.BillingAddress).HasMaxLength(500);
        builder.Property(a => a.BankAccount).HasMaxLength(50);
        builder.Property(a => a.InvoiceCurrency).HasMaxLength(3);

        builder.HasOne<CompanyType>().WithMany().HasForeignKey(a => a.CompanyTypeCode);

        AuditConfiguration.Apply<Company, string>(builder);
        builder.HasAuditType<Company, CompanyAudit>();
    }
}
