using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}