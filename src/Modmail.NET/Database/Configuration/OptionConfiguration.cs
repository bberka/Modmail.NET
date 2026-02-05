using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.HasKey(x => x.ServerId);

        builder.Property(x => x.ServerId)
            .ValueGeneratedNever();
    }
}