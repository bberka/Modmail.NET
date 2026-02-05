using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();
    }
}