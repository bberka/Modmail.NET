using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TeamUserConfiguration : IEntityTypeConfiguration<TeamUser>
{
  public void Configure(EntityTypeBuilder<TeamUser> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();
  }
}