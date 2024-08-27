using Microsoft.EntityFrameworkCore;
using Modmail.NET.Entities;

namespace Modmail.NET.Database;

public sealed class ModmailDbContext : DbContext
{
  public ModmailDbContext() : base() { }
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
    optionsBuilder.UseSqlServer(BotConfig.This.DbConnectionString);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModmailDbContext).Assembly);
  }
}