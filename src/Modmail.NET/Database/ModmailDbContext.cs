using Microsoft.EntityFrameworkCore;
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
  public DbSet<TicketType> TicketTypes { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    switch (BotConfig.This.DbType) {
      case DbType.Sqlite:
        optionsBuilder.UseSqlite(BotConfig.This.DbConnectionString);
        break;
      case DbType.Postgres:
        optionsBuilder.UseNpgsql(BotConfig.This.DbConnectionString);
        break;
      case DbType.MsSql:
        optionsBuilder.UseSqlServer(BotConfig.This.DbConnectionString);
        break;
      case DbType.MySql:
        optionsBuilder.UseMySql(BotConfig.This.DbConnectionString, ServerVersion.AutoDetect(BotConfig.This.DbConnectionString));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Ticket>()
                .Navigation(x => x.GuildOption)
                .AutoInclude();

    modelBuilder.Entity<Ticket>()
                .Navigation(x => x.TicketType)
                .AutoInclude();

    modelBuilder.Entity<Ticket>()
                .Navigation(x => x.OpenerUserInfo)
                .AutoInclude();

    modelBuilder.Entity<Ticket>()
                .Navigation(x => x.CloserUserInfo)
                .AutoInclude();
    modelBuilder.Entity<GuildTeam>()
                .Navigation(x => x.GuildTeamMembers)
                .AutoInclude();
  }
}