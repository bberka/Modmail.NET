using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class DiscordUserInfoConfiguration : IEntityTypeConfiguration<UserInformation>
{
  public void Configure(EntityTypeBuilder<UserInformation> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedNever();
  }
}