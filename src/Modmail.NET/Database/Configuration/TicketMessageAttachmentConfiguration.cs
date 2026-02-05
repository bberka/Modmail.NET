using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketMessageAttachmentConfiguration : IEntityTypeConfiguration<TicketMessageAttachment>
{
    public void Configure(EntityTypeBuilder<TicketMessageAttachment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne<TicketMessage>()
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.TicketMessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}