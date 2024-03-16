﻿using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Entities;

namespace Modmail.NET.Database.Services;

public class DbService : IDbService
{
  private readonly ModmailDbContext _dbContext;

  public DbService(ModmailDbContext dbContext) {
    _dbContext = dbContext;
  }

  public async Task<TicketOption?> GetOptionAsync(ulong guildId) {
    return await _dbContext.TicketOptions.FirstOrDefaultAsync(x => x.GuildId == guildId);
  }

  public async Task<Ticket?> GetActiveTicketAsync(ulong discordUserId) {
    return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.DiscordUserId == discordUserId && !x.ClosedDate.HasValue);
  }

  public async Task<Ticket?> GetActiveTicketAsync(Guid modmailId) {
    return await _dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == modmailId && !x.ClosedDate.HasValue);
  }

  public async Task<ulong> GetLogChannelIdAsync(ulong guildId) {
    return await _dbContext.TicketOptions.Where(x => x.GuildId == guildId).Select(x => x.LogChannelId).FirstOrDefaultAsync();
  }

  public async Task UpdateTicketOptionAsync(TicketOption option) {
    _dbContext.TicketOptions.Update(option);
    await _dbContext.SaveChangesAsync();
  }

  public async Task AddTicketOptionAsync(TicketOption option) {
    await _dbContext.TicketOptions.AddAsync(option);
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
}