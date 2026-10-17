using Aero.Flights.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.Flights.Infrastructure.Persistence.Configurations.Messaging;

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessage", "ops");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.Source).IsRequired().HasMaxLength(100);
        builder.Property(m => m.MessageType).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Payload).IsRequired();
        builder.Property(m => m.LastError).HasMaxLength(1000);

        builder.HasIndex(m => m.ProcessedAt);

        // Partial index for the processor's unprocessed scan.
        builder.HasIndex(m => m.Id)
            .HasDatabaseName("IX_InboxMessage_Unprocessed")
            .HasFilter("\"ProcessedAt\" IS NULL");
    }
}
