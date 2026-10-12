using Domain.Aggregates.Bags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class ArrivalBagEventConfiguration : IEntityTypeConfiguration<ArrivalBagEvent>
{
    public void Configure(EntityTypeBuilder<ArrivalBagEvent> builder)
    {
        builder.ToTable("ArrivalBagEvents", "bags");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).UseIdentityColumn();

        builder.Property(b => b.BagId).IsRequired();

        builder.Property(b => b.DeviceId);

        builder.Property(b => b.EventId).HasMaxLength(32).IsRequired();

        builder.Property(b => b.Description).HasMaxLength(50);

        builder.Property(b => b.UserName).HasMaxLength(20);

        builder.Property(b => b.EventTime).IsRequired();

        builder.HasOne(b => b.ArrivalBag).WithMany().HasForeignKey(b => b.BagId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.BagId).IncludeProperties(["EventId", "Description", "UserName", "EventTime"]);

    }
}