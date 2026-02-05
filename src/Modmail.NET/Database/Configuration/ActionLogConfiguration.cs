using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class ActionLogConfiguration : IEntityTypeConfiguration<ActionLog>
{
    public void Configure(EntityTypeBuilder<ActionLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Event);
    }
}