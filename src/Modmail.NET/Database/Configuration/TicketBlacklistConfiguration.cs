﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Database.Configuration;

public class TicketBlacklistConfiguration : IEntityTypeConfiguration<Blacklist>
{
  public void Configure(EntityTypeBuilder<Blacklist> builder) {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
           .ValueGeneratedOnAdd();

    builder.HasIndex(x => x.UserId)
           .IsUnique();

    builder.HasOne(x => x.User)
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}