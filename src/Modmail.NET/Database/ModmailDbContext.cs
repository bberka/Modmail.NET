using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Entities;
using SmartEnum.EFCore;

namespace Modmail.NET.Database;

public class ModmailDbContext : DbContext
{
  public ModmailDbContext() { }
  public ModmailDbContext(DbContextOptions<ModmailDbContext> options) : base(options) { }
  public required DbSet<Ticket> Tickets { get; set; }
  public required DbSet<TicketMessageAttachment> MessageAttachments { get; set; }
  public required DbSet<TicketMessage> Messages { get; set; }
  public required DbSet<TicketMessageHistory> MessageHistory { get; set; }
  public required DbSet<Option> Options { get; set; }
  public required DbSet<Team> Teams { get; set; }
  public required DbSet<TeamUser> TeamUsers { get; set; }
  public required DbSet<TeamPermission> TeamPermissions { get; set; }
  public required DbSet<TicketNote> TicketNotes { get; set; }
  public required DbSet<UserInformation> UserInformation { get; set; }
  public required DbSet<Blacklist> Blacklists { get; set; }
  public required DbSet<TicketType> TicketTypes { get; set; }
  public required DbSet<Statistic> Statistics { get; set; }
  public required DbSet<Tag> Tags { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModmailDbContext).Assembly);
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
    configurationBuilder.ConfigureSmartEnum();
  }
}