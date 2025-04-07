using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Entities;
using SmartEnum.EFCore;

namespace Modmail.NET.Database;

public class ModmailDbContext : DbContext
{
  public ModmailDbContext() { }
  public ModmailDbContext(DbContextOptions<ModmailDbContext> options) : base(options) { }
  public DbSet<Ticket> Tickets { get; set; } = null!;
  public DbSet<TicketMessageAttachment> TicketMessageAttachments { get; set; } = null!;
  public DbSet<TicketMessage> TicketMessages { get; set; } = null!;
  public DbSet<TicketMessageHistory> TicketMessageHistory { get; set; } = null!;
  public DbSet<GuildOption> GuildOptions { get; set; } = null!;
  public DbSet<GuildTeam> GuildTeams { get; set; } = null!;
  public DbSet<GuildTeamMember> GuildTeamMembers { get; set; } = null!;
  public DbSet<TicketNote> TicketNotes { get; set; } = null!;
  public DbSet<DiscordUserInfo> DiscordUserInfos { get; set; } = null!;
  public DbSet<TicketBlacklist> TicketBlacklists { get; set; } = null!;
  public DbSet<TicketType> TicketTypes { get; set; } = null!;
  public DbSet<Statistic> Statistics { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModmailDbContext).Assembly);
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
    configurationBuilder.ConfigureSmartEnum();
  }
}