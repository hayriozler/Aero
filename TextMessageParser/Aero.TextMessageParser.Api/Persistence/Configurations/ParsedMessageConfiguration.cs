using Aero.TextMessageParser.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aero.TextMessageParser.Api.Persistence.Configurations;

internal sealed class ParsedMessageConfiguration : IEntityTypeConfiguration<ParsedMessage>
{
    public void Configure(EntityTypeBuilder<ParsedMessage> builder)
    {
        builder.ToTable("ParsedMessages", "messages");
        builder.HasKey(e => e.Id).HasName("PK_ParsedMessageId");
        builder.Property(e => e.Id).HasColumnType("BIGINT");
        builder.Property(e => e.TextMessageId).HasColumnType("BIGINT");
        builder.Property(e => e.MessageType).HasMaxLength(10).IsRequired();
        builder.Property(e => e.SourceAirport).HasMaxLength(3).IsRequired();
        builder.Property(e => e.AirlineCode).HasMaxLength(3).IsRequired();
        builder.Property(e => e.FlightNumber).HasMaxLength(6).IsRequired();
        builder.Property(e => e.FlightDate).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Destination).HasMaxLength(3);
        builder.Property(e => e.PassengerName).HasMaxLength(100);
        builder.Property(e => e.PassengerSequenceNumber).HasMaxLength(10);
        builder.Property(e => e.PassengerSecurityNumber).HasMaxLength(10);
        builder.Property(e => e.SeatNumber).HasMaxLength(5);
        builder.Property(e => e.WeightIndicator).HasMaxLength(10);
        builder.Property(e => e.CheckedBagCount).HasMaxLength(10);
        builder.Property(e => e.CheckedWeight).HasMaxLength(10);
        builder.Property(e => e.UncheckedWeight).HasMaxLength(10);
        builder.Property(e => e.WeightUnit).HasMaxLength(10);
        builder.Property(e => e.BagLength).HasMaxLength(10);
        builder.Property(e => e.BagWidth).HasMaxLength(10);
        builder.Property(e => e.BagHeight).HasMaxLength(10);
        builder.Property(e => e.BaggageTypeCode).HasMaxLength(10);
        builder.Property(e => e.CreatedAtUtc).HasColumnType("TIMESTAMPTZ").IsRequired();
        builder.Property(e => e.ConsumedAt).HasColumnType("TIMESTAMPTZ");
        builder.HasIndex(e => e.Id).HasDatabaseName("IX_ParsedMessages_Unconsumed").HasFilter("\"IsConsumed\" = false");

        builder.HasIndex(e => e.TextMessageId).IsUnique();
        builder.HasIndex(e => new { e.AirlineCode, e.FlightNumber, e.FlightDate });

        builder.HasOne<TextMessage>().WithOne().HasForeignKey<ParsedMessage>(e => e.TextMessageId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.BagTags).WithOne().HasForeignKey(e => e.ParsedMessageId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Onwards).WithOne().HasForeignKey(e => e.ParsedMessageId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ParsedBagTagConfiguration : IEntityTypeConfiguration<ParsedBagTag>
{
    public void Configure(EntityTypeBuilder<ParsedBagTag> builder)
    {
        builder.ToTable("ParsedBagTags", "messages");
        builder.HasKey(e => e.Id).HasName("PK_ParsedBagTagId");
        builder.Property(e => e.Id).HasColumnType("BIGINT");
        builder.Property(e => e.ParsedMessageId).HasColumnType("BIGINT");
        builder.Property(e => e.BagTag).HasMaxLength(10).IsRequired();
        builder.HasIndex(e => e.BagTag);
        builder.HasIndex(e => e.ParsedMessageId);
    }
}

internal sealed class ParsedOnwardFlightConfiguration : IEntityTypeConfiguration<ParsedOnwardFlight>
{
    public void Configure(EntityTypeBuilder<ParsedOnwardFlight> builder)
    {
        builder.ToTable("ParsedOnwardFlights", "messages");
        builder.HasKey(e => e.Id).HasName("PK_ParsedOnwardFlightId");
        builder.Property(e => e.Id).HasColumnType("BIGINT");
        builder.Property(e => e.ParsedMessageId).HasColumnType("BIGINT");
        builder.Property(e => e.AirlineCode).HasMaxLength(3).IsRequired();
        builder.Property(e => e.FlightNumber).HasMaxLength(6).IsRequired();
        builder.Property(e => e.FlightDate).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Destination).HasMaxLength(3).IsRequired();
        builder.HasIndex(e => e.ParsedMessageId);
    }
}
