using IataText.Parser.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.TextMessages;

internal sealed class TextMessageErrorConfiguration : IEntityTypeConfiguration<TextMessageError>
{
    public void Configure(EntityTypeBuilder<TextMessageError> builder)
    {
        var table = builder.ToTable("MessageErrors", "messages");

        builder.HasKey(e => e.Id).HasName("PK_MessageErrorId");

        builder.Property(e => e.Id)
        .HasColumnType("INTEGER")
          .HasColumnOrder(0);

        builder.Property(e => e.MessageId)
       .HasColumnType("BIGINT")
        .HasColumnOrder(1);

        builder.Property(e => e.ErrorCode)
        .HasColumnType("VARCHAR")
        .HasMaxLength(50)
        .HasColumnOrder(2);

        builder.Property(e => e.ErrorDescription)
      .HasColumnType("VARCHAR")
      .HasMaxLength(250)
      .HasColumnOrder(3)
      .IsRequired(false);

        builder.Property(e => e.RecordDateTime)
       .HasColumnType("TIMESTAMPTZ")
       .IsRequired()
       .HasColumnOrder(4);
    }
}