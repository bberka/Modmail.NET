using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketMessageHistoryConfiguration : IEntityTypeConfiguration<TicketMessageHistory>
{
    public void Configure(EntityTypeBuilder<TicketMessageHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne(hm => hm.TicketMessage)
            .WithMany(m => m.History)
            .HasForeignKey(hm => hm.TicketMessageId);
    }
}