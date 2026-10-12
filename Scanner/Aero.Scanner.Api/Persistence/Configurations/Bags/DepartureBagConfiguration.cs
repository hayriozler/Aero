using Aero.Scanner.Domain.Aggregates.Bags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Scanner.Api.Persistence.Configurations.Bags;

internal sealed class DepartureBagConfiguration : IEntityTypeConfiguration<DepartureBag>
{
    public void Configure(EntityTypeBuilder<DepartureBag> builder)
    {
        builder.ToTable("DepartureBags", "bags");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).UseIdentityColumn();

        builder.Property(b => b.TagNumber).HasMaxLength(10).IsRequired();

        builder.Property(b => b.Class).IsRequired();

        builder.Property(b => b.FlightPassengerId).IsRequired();
        builder.Property(b => b.AckRequest);
        builder.Property(b => b.SourceIndicator).HasMaxLength(1);
        builder.Property(b => b.SourceAirportCode).HasMaxLength(3);

        builder.Property(b => b.ContainerId);
        builder.Property(b => b.HhtId);
        builder.Property(b => b.DepartureBaggageStatus).HasConversion<short>().IsRequired();
        builder.Property(b => b.UserName).HasMaxLength(20);
        builder.Property(b => b.IsDeleted).IsRequired();

        builder.Property(b => b.WeightIndicator).HasMaxLength(1);
        builder.Property(b => b.CheckedWeight).HasMaxLength(5);
        builder.Property(b => b.OffloadReason);
        builder.Property(b => b.RushReason).HasMaxLength(10);
        builder.Property(f => f.UpdatedAt).IsRequired();

        builder.HasOne(b => b.FlightPassenger)
            .WithMany()
            .HasForeignKey(b => b.FlightPassengerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasIndex(b => new { b.FlightPassengerId, b.TagNumber });
    }
}