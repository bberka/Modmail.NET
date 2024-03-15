using Microsoft.EntityFrameworkCore;
using Modmail.NET.Cache;
using Modmail.NET.Entities;

namespace Modmail.NET.Database;

public partial class ModmailDbContext
{
  public async Task<TicketOption> GetOptionAsync(ulong guildId) {
    const int cacheTimeSeconds = 60;
    var cacheKey = $"TicketOption:{guildId}";
    var data = DatabaseCache.This.Get(cacheKey);
    if (data != null) return (TicketOption)data;
    data = await TicketOptions.FirstOrDefaultAsync(x => x.GuildId == guildId);
    if (data is null) throw new Exception("TicketOption not found for guild");
    DatabaseCache.This.Set(cacheKey, data, TimeSpan.FromSeconds(cacheTimeSeconds));
    return (TicketOption)data;
  }

  public async Task<Ticket?> GetActiveModmailAsync(ulong discordUserId) {
    return await Tickets.FirstOrDefaultAsync(x => x.DiscordUserId == discordUserId && !x.ClosedDate.HasValue);
  }

  public async Task<Ticket?> GetActiveModmailAsync(Guid modmailId) {
    return await Tickets.FindAsync(modmailId);
  }
}