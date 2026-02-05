using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TeamPermissionConfiguration : IEntityTypeConfiguration<TeamPermission>
{
    public void Configure(EntityTypeBuilder<TeamPermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new { x.AuthPolicy, x.TeamId })
            .IsUnique();
    }
}