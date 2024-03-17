using Modmail.NET.Entities;

namespace Modmail.NET.Abstract.Services;

public interface IDbService
{
  Task<GuildOption?> GetOptionAsync(ulong guildId);
  Task<Ticket?> GetActiveTicketAsync(ulong discordUserId);
  Task<Ticket?> GetActiveTicketAsync(Guid ticketId);
  Task<ulong> GetLogChannelIdAsync(ulong guildId);
  Task UpdateTicketOptionAsync(GuildOption option);
  Task AddTicketOptionAsync(GuildOption option);
  Task UpdateTicketAsync(Ticket ticket);
  Task AddTicketAsync(Ticket ticket);
  Task AddMessageLog(TicketMessage dbMessageLog);
  Task<List<Tag>> GetTagsAsync(ulong guildId);
  Task<Tag?> GetTagAsync(ulong guildId, string key);
  Task AddTagAsync(Tag tag);
  Task RemoveTagAsync(Tag tag);
  Task<List<GuildTeam>> GetTeamsAsync(ulong guildId);
  Task AddTeamAsync(GuildTeam team);
  Task RemoveTeamAsync(GuildTeam team);
  Task UpdateTeamAsync(GuildTeam team);

  Task<GuildTeam?> GetTeamByIdAsync(ulong guildId, Guid id);
  Task<GuildTeam?> GetTeamByNameAsync(ulong guildId, string name);
  Task<GuildTeam?> GetTeamByIndexAsync(ulong guildId, int index);
}