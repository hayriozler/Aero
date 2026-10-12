using AeroFlight.Domain.Metadatas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Metadata;

internal sealed class CompanyContactConfiguration : IEntityTypeConfiguration<CompanyContact>
{
    public void Configure(EntityTypeBuilder<CompanyContact> builder)
    {
        builder.ToTable("CompanyContact", "metadata");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CompanyCode).IsRequired().HasMaxLength(10).HasColumnType("citext");
        builder.Property(c => c.ContactType).HasConversion<string>().IsRequired().HasMaxLength(10);
        builder.Property(c => c.Value).IsRequired().HasMaxLength(100);

        builder.HasOne<Company>().WithMany().HasForeignKey(c => c.CompanyCode).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.CompanyCode);
    }
}
