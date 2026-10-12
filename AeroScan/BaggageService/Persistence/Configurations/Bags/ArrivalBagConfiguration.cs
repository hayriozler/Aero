using Domain.Aggregates.Bags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class ArrivalBagConfiguration : IEntityTypeConfiguration<ArrivalBag>
{
    public void Configure(EntityTypeBuilder<ArrivalBag> builder)
    {
        
        builder.ToTable("ArrivalBags", "bags");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).UseIdentityColumn();

        builder.Property(b => b.TagNumber).HasMaxLength(10).IsRequired();

        builder.Property(b => b.Class).HasConversion<short>().IsRequired();

        builder.Property(b => b.FlightPassengerId).IsRequired();
        builder.Property(b => b.SourceIndicator).HasMaxLength(1);
        builder.Property(b => b.SourceAirportCode).HasMaxLength(3);

        builder.Property(b => b.CheckedWeight).HasMaxLength(5);

        builder.Property(b => b.HhtId);
        builder.Property(b => b.ArrivalBaggageStatus).HasConversion<short>().IsRequired();
        builder.Property(b => b.UserName).HasMaxLength(20);
        builder.Property(b => b.IsDeleted).IsRequired();
        builder.Property(f => f.UpdatedAt).IsRequired();

        builder.HasOne(b => b.FlightPassenger)
            .WithMany()
            .HasForeignKey(b => b.FlightPassengerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.FlightPassengerId).IncludeProperties(["TagNumber"]);
        builder.HasIndex(b => b.TagNumber).IncludeProperties(["FlightPassengerId"]);
    }
}
