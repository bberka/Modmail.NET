using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public class GuildTeamMemberConfiguration : IEntityTypeConfiguration<GuildTeamMember>
{
  public void Configure(EntityTypeBuilder<GuildTeamMember> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();
  }
}