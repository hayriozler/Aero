using Domain.Aggregates.Bags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.Bags;

internal sealed class DepartureBagEventConfiguration : IEntityTypeConfiguration<DepartureBagEvent>
{
    public void Configure(EntityTypeBuilder<DepartureBagEvent> builder)
    {
        builder.ToTable("DepartureBagEvents", "bags");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).UseIdentityColumn();

        builder.Property(b => b.BagId).IsRequired();

        builder.Property(b => b.MessageId);

        builder.Property(b => b.DeviceId);

        builder.Property(b => b.EventId).HasMaxLength(32).IsRequired();

        builder.Property(b => b.Description).HasMaxLength(64);

        builder.Property(b => b.UserName).HasMaxLength(20);

        builder.Property(b => b.EventTime).IsRequired();

        builder.HasOne(b => b.DepartureBag).WithMany().HasForeignKey(b => b.BagId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => b.BagId).IncludeProperties(["EventId", "Description", "UserName", "EventTime"]);

    }
}
