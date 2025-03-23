using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
  public void Configure(EntityTypeBuilder<Ticket> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();

    builder.HasOne(x => x.OpenerUser)
           .WithMany(x => x.OpenedTickets)
           .HasForeignKey(x => x.OpenerUserId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(x => x.CloserUser)
           .WithMany(x => x.ClosedTickets)
           .HasForeignKey(x => x.CloserUserId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(x => x.AssignedUser)
           .WithMany(x => x.AssignedTickets)
           .HasForeignKey(x => x.AssignedUserId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(x => x.TicketType)
           .WithMany()
           .HasForeignKey(x => x.TicketTypeId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.Navigation(x => x.TicketType)
           .AutoInclude();

    builder.Navigation(x => x.OpenerUser)
           .AutoInclude();
    builder.Navigation(x => x.CloserUser)
           .AutoInclude();
  }
}