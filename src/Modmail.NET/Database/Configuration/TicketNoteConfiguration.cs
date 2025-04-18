﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketNoteConfiguration : IEntityTypeConfiguration<TicketNote>
{
  public void Configure(EntityTypeBuilder<TicketNote> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();

    builder.HasOne<Ticket>()
           .WithMany(x => x.TicketNotes)
           .HasForeignKey(x => x.TicketId)
           .OnDelete(DeleteBehavior.Cascade);
  }
}