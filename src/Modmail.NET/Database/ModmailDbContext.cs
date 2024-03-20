﻿using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;

namespace Modmail.NET.Database;

public class ModmailDbContext : DbContext
{
  public DbSet<Ticket> Tickets { get; set; }
  public DbSet<TicketMessageAttachment> TicketMessageAttachments { get; set; }
  public DbSet<TicketMessage> TicketMessages { get; set; }

  public DbSet<GuildOption> GuildOptions { get; set; }
  public DbSet<GuildTeam> GuildTeams { get; set; }
  public DbSet<GuildTeamMember> GuildTeamMembers { get; set; }
  public DbSet<TicketNote> TicketNotes { get; set; }
  public DbSet<DiscordUserInfo> DiscordUserInfos { get; set; }
  public DbSet<TicketBlacklist> TicketBlacklists { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    switch (MMConfig.This.DbType) {
      case DbType.Sqlite:
        optionsBuilder.UseSqlite(MMConfig.This.DbConnectionString);
        break;
      case DbType.Postgres:
        optionsBuilder.UseNpgsql(MMConfig.This.DbConnectionString);
        break;
      case DbType.MsSql:
        optionsBuilder.UseSqlServer(MMConfig.This.DbConnectionString);
        break;
      case DbType.MySql:
        optionsBuilder.UseMySql(MMConfig.This.DbConnectionString, ServerVersion.AutoDetect(MMConfig.This.DbConnectionString));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}