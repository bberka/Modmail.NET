using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class GuildOptionConfiguration : IEntityTypeConfiguration<GuildOption>
{
  public void Configure(EntityTypeBuilder<GuildOption> builder) {
    builder.HasKey(x => x.GuildId);

    builder.Property(x => x.GuildId)
           .ValueGeneratedNever();
  }
}