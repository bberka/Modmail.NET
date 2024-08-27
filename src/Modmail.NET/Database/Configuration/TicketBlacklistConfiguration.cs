using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public sealed class TicketBlacklistConfiguration : IEntityTypeConfiguration<TicketBlacklist>
{
  public void Configure(EntityTypeBuilder<TicketBlacklist> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();

    builder.HasIndex(x => x.DiscordUserId)
           .IsUnique();

    builder.HasOne<DiscordUserInfo>()
           .WithOne()
           .HasForeignKey(nameof(TicketBlacklist), nameof(TicketBlacklist.DiscordUserId))
           .OnDelete(DeleteBehavior.Cascade);
  }
}