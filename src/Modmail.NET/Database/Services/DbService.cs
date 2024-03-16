using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Services;

public class DbService : IDbService
{
  private readonly ModmailDbContext _dbContext;

  public DbService(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<GuildOption?> GetOptionAsync(ulong guildId) {
    return await _dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == guildId);
  }

  public async Task<Ticket?> GetActiveTicketAsync(ulong discordUserId) {
    return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.DiscordUserId == discordUserId && !x.ClosedDate.HasValue);
  }

  public async Task<Ticket?> GetActiveTicketAsync(Guid modmailId) {
    return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == modmailId && !x.ClosedDate.HasValue);
  }

  public async Task<ulong> GetLogChannelIdAsync(ulong guildId) {
    return await _dbContext.GuildOptions.Where(x => x.GuildId == guildId).Select(x => x.LogChannelId).FirstOrDefaultAsync();
  }

  public async Task UpdateTicketOptionAsync(GuildOption option) {
    _dbContext.GuildOptions.Update(option);
    await _dbContext.SaveChangesAsync();
  }

  public async Task AddTicketOptionAsync(GuildOption option) {
    await _dbContext.GuildOptions.AddAsync(option);
    await _dbContext.SaveChangesAsync();
  }

  public async Task UpdateTicketAsync(Ticket ticket) {
    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync();
  }

  public async Task AddTicketAsync(Ticket ticket) {
    await _dbContext.Tickets.AddAsync(ticket);
    await _dbContext.SaveChangesAsync();
  }

  public async Task AddMessageLog(TicketMessage dbMessageLog) {
    await _dbContext.TicketMessages.AddAsync(dbMessageLog);
    await _dbContext.SaveChangesAsync();
  }
  
  public async Task<List<Tag>> GetTagsAsync(ulong guildId) {
    return await _dbContext.Tags.Where(x => x.GuildId == guildId).ToListAsync();
  }

  public async Task<Tag?> GetTagAsync(ulong guildId, string key) {
    return await _dbContext.Tags.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Key == key);
  }
}