using AeroFlight.Domain.Audit.MetaData;
using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

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
