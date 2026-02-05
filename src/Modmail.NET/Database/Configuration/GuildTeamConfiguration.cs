using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public sealed class GuildTeamConfiguration : IEntityTypeConfiguration<GuildTeam>
{
    public void Configure(EntityTypeBuilder<GuildTeam> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.HasMany(x => x.GuildTeamMembers)
            .WithOne(x => x.GuildTeam)
            .HasForeignKey(x => x.GuildTeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}