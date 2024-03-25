using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

[Index(nameof(DiscordUserId), nameof(GuildId), IsUnique = true)]
public class TicketBlacklist
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string? Reason { get; set; }

  [ForeignKey(nameof(DiscordUserInfo))]
  public ulong DiscordUserId { get; set; }

  [ForeignKey(nameof(GuildOption))]
  public ulong GuildId { get; set; }

  //FK
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual GuildOption GuildOption { get; set; }

  public static async Task<bool> IsBlacklistedAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserId == userId);
  }


  public static async Task<List<TicketBlacklist>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketBlacklists.ToListAsync();
  }

  public static async Task<TicketBlacklist?> GetAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.TicketBlacklists.FirstOrDefaultAsync(x => x.DiscordUserId == userId);
  }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketBlacklists.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task RemoveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.TicketBlacklists.Remove(this);
    await dbContext.SaveChangesAsync();
  }
}