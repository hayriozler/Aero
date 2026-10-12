using IataText.Parser.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.TextMessages;

internal sealed class TextMessageConfiguration : IEntityTypeConfiguration<TextMessage>
{
    public void Configure(EntityTypeBuilder<TextMessage> builder)
    {
        var table = builder.ToTable("TextMessages", "messages");

        builder.HasKey(e => e.Id).HasName("PK_TextMessageId");

        builder.Property(e => e.Id)
        .HasColumnType("BIGINT")
        .HasColumnOrder(0);
        builder.OwnsOne(e => e.Header, header =>
        {
            header.Property(e => e.Identifier)
            .HasColumnType("VARCHAR")
            .HasMaxLength(3)
            .HasColumnOrder(1);

            header.Property(e => e.SecondaryIdentifier)
           .HasColumnType("VARCHAR")
           .HasMaxLength(3)
           .HasColumnOrder(2);

            header.Property(e => e.ChangeOfStatus)
           .HasColumnType("VARCHAR")
           .HasMaxLength(3)
           .HasColumnOrder(3);

            header.HasIndex(x => new { x.Identifier, x.SecondaryIdentifier, x.ChangeOfStatus });

        });
        builder.OwnsOne(e => e.Footer, footer => {
            footer.Property(e => e.EndIdentifier)
            .HasColumnType("VARCHAR")
            .HasMaxLength(6)
            .HasColumnOrder(4);
        });
        builder.Property(e => e.Message)
       .HasColumnType("TEXT")
       .HasMaxLength(4000)
       .HasColumnOrder(5);

        builder.Property(e => e.Status)
        .HasColumnType("TEXT")
        .HasMaxLength(10)
        .HasColumnOrder(6);

        builder.Property(e => e.ProcessingStartedAt)
            .HasColumnType("TIMESTAMPTZ")
            .HasColumnOrder(8);


        builder.Property(e => e.Completed)
      .HasColumnType("boolean")
      .HasColumnOrder(9);

        builder.Property(e => e.ProcessDateTime)
           .HasColumnType("TIMESTAMPTZ")
           .IsRequired(false)
           .HasColumnOrder(10);

        builder.Property(e => e.RecordDateTime)
          .HasColumnType("TIMESTAMPTZ")
          .IsRequired()
          .HasColumnOrder(11);

        builder.HasIndex(x => new { x.Status, x.Completed });

    }
}
