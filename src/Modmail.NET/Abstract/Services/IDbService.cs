using Modmail.NET.Entities;

namespace Modmail.NET.Abstract.Services;

public interface IDbService
{
  Task<TicketOption?> GetOptionAsync(ulong guildId);
  Task<Ticket?> GetActiveTicketAsync(ulong discordUserId);
  Task<Ticket?> GetActiveTicketAsync(Guid ticketId);
  Task<ulong> GetLogChannelIdAsync(ulong guildId);
  Task UpdateTicketOptionAsync(TicketOption option);
  Task AddTicketOptionAsync(TicketOption option);
  Task UpdateTicketAsync(Ticket ticket);
  Task AddTicketAsync(Ticket ticket);
  Task AddMessageLog(TicketMessage dbMessageLog);
  Task<List<Tag>> GetTagsAsync(ulong guildId);
  Task<Tag?> GetTagAsync(ulong guildId, string key);
}