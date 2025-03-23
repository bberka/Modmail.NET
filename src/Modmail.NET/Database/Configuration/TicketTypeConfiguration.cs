using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
  public void Configure(EntityTypeBuilder<TicketType> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();
  }
}