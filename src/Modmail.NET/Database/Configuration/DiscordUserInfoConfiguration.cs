using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public sealed class DiscordUserInfoConfiguration : IEntityTypeConfiguration<DiscordUserInfo>
{
  public void Configure(EntityTypeBuilder<DiscordUserInfo> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedNever();
  }
}