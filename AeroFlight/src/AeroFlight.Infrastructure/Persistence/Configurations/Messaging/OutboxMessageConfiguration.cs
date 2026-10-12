using AeroFlight.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AeroFlight.Infrastructure.Persistence.Configurations.Messaging;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage", "ops");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();

        builder.Property(m => m.Exchange).IsRequired().HasMaxLength(100);
        builder.Property(m => m.RoutingKey).IsRequired().HasMaxLength(100);
        builder.Property(m => m.MessageType).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Payload).IsRequired();
        builder.Property(m => m.LastError).HasMaxLength(1000);

        builder.HasIndex(m => m.PublishedAt);

        // Partial index for the dispatcher's unpublished scan.
        builder.HasIndex(m => m.Id)
            .HasDatabaseName("IX_OutboxMessage_Unpublished")
            .HasFilter("\"PublishedAt\" IS NULL");
    }
}
