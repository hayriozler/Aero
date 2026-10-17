using Aero.Flights.Domain.Audit.MetaData;
using Aero.Flights.Domain.Metadatas;
using Aero.Flights.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CompanyTypeConfiguration : IEntityTypeConfiguration<CompanyType>
{
    public void Configure(EntityTypeBuilder<CompanyType> builder)
    {
        builder.ToTable("CompanyType", "metadata");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired().HasMaxLength(20).HasColumnType("citext").HasColumnName("Code");
        builder.Property(c => c.Description).IsRequired().HasMaxLength(50);

        AuditConfiguration.Apply<CompanyType, string>(builder);
        builder.HasAuditType<CompanyType, CompanyTypeAudit>();
    }
}
