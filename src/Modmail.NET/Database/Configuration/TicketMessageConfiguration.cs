using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
  public void Configure(EntityTypeBuilder<TicketMessage> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();

    builder.HasOne<DiscordUserInfo>()
           .WithMany()
           .HasForeignKey(x => x.SenderUserId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Ticket>()
           .WithMany(x => x.Messages)
           .HasForeignKey(x => x.TicketId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}