using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

public class GuildOption
{
  [Key]
  public ulong GuildId { get; set; }


  public bool IsAutoUpdateGuildInformation { get; set; } = true;
  public string Name { get; set; } = "Modmail";
  public string IconUrl { get; set; } = "";

  public string? BannerUrl { get; set; }
  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; } = DateTime.UtcNow;

  public bool IsSensitiveLogging { get; set; } = true;

  public string GreetingMessage { get; set; }
    = "Thank you for reaching out to our team, we'll reply to you as soon as possible. Please help us speed up this process by describing your request in detail.";

  public string ClosingMessage { get; set; } = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";

  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }

  public virtual List<GuildTeam> GuildTeams { get; set; }

  public virtual List<Ticket> Tickets { get; set; }
  public virtual List<TicketBlacklist> TicketBlacklists { get; set; }


  public static async Task<GuildOption?> GetAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == MMConfig.This.MainServerId);
  }

  public async Task UpdateAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildOptions.Update(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<ulong> GetLogChannelIdAsync(ulong guildId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.Where(x => x.GuildId == guildId).Select(x => x.LogChannelId).FirstOrDefaultAsync();
  }

  public async Task AddAsync() {
    await DeleteAllAsync();
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildOptions.Add(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<bool> Any() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildOptions.AnyAsync();
  }

  private static async Task DeleteAllAsync() {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var options = await dbContext.GuildOptions.ToListAsync();
    dbContext.GuildOptions.RemoveRange(options);
    await dbContext.SaveChangesAsync();
  }
}