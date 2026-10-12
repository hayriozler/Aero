using IataText.Parser.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaggageService.Persistence.Configurations.TextMessages;

internal sealed class ElementErrorConfiguration : IEntityTypeConfiguration<ElementError>
{
    public void Configure(EntityTypeBuilder<ElementError> builder)
    {
        builder.ToTable("ElementErrors","messages");

        builder.HasKey(e => e.Id).HasName("PK_ElementErrorId");

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
            .IsRequired(false)
            .HasColumnOrder(3);

        builder.Property(e => e.RecordDateTime)
            .HasColumnType("TIMESTAMPTZ")
            .IsRequired()
            .HasColumnOrder(4);
    
    }
}
