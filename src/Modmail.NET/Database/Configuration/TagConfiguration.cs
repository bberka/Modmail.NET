using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modmail.NET.Database.Configuration;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Navigation(x => x.Embed)
            .AutoInclude();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(e => e.Name)
            .HasConversion(v => Tag.GetTagName(v), v => v);
    }
}