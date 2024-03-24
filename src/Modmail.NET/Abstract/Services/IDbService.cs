﻿using Modmail.NET.Entities;
using Modmail.NET.Models;
using Modmail.NET.Static;

namespace Modmail.NET.Abstract.Services;

public interface IDbService
{
  Task<GuildOption?> GetOptionAsync(ulong guildId);
  Task<Ticket?> GetActiveTicketAsync(ulong discordUserId);
  Task<Ticket?> GetActiveTicketAsync(Guid ticketId);
  Task<ulong> GetLogChannelIdAsync(ulong guildId);
  Task UpdateGuildOptionAsync(GuildOption option);
  Task AddGuildOptionAsync(GuildOption option);
  Task UpdateTicketAsync(Ticket ticket);
  Task AddTicketAsync(Ticket ticket);

  Task AddMessageLog(TicketMessage dbMessageLog);
  Task<List<GuildTeam>> GetTeamsAsync(ulong guildId);
  Task AddTeamAsync(GuildTeam team);
  Task RemoveTeamAsync(GuildTeam team);
  Task UpdateTeamAsync(GuildTeam team);

  Task<GuildTeam?> GetTeamByIdAsync(ulong guildId, Guid id);
  Task<GuildTeam?> GetTeamByNameAsync(ulong guildId, string name);
  Task<GuildTeam?> GetTeamByIndexAsync(ulong guildId, int index);
  Task AddNoteAsync(TicketNote noteEntity);
  Task<TeamPermissionLevel?> GetPermissionLevelAsync(ulong userId, ulong guildId, List<ulong> roleIdList);

  Task<List<PermissionInfo>> GetPermissionInfoAsync(ulong guildId);
  Task<List<PermissionInfo>> GetPermissionInfoOrHigherAsync(ulong guildId, TeamPermissionLevel levelOrHigher);
  Task UpdateUserInfoAsync(DiscordUserInfo dcUserInfo);
  Task<bool> GetUserBlacklistStatus(ulong authorId);
  Task AddBlacklistAsync(ulong userId, ulong guildId, string? reason);
  Task RemoveBlacklistAsync(ulong isBlocked);
  Task<List<ulong>> GetBlacklistedUsersAsync(ulong guildId);
  Task<Ticket> GetClosedTicketAsync(Guid ticketId);
  Task AddFeedbackAsync(Guid ticketId, int starCount, string textInput);
  Task<bool> AnyServerSetupAsync();
  Task<bool> IsUserInAnyTeamAsync(ulong memberId);
  Task<bool> IsRoleInAnyTeamAsync(ulong roleId);
  Task AddTicketTypeAsync(TicketType ticketType);
  Task<TicketType?> GetTicketTypeByIdAsync(Guid id);
  Task<TicketType?> GetTicketTypeByKeyAsync(string key);
  Task<TicketType?> GetTicketTypeByNameAsync(string name);
  Task<bool> TicketTypeExists(string relatedContent);
  Task<List<TicketType>> GetEnabledTicketTypesAsync();
  Task RemoveTicketTypeAsync(TicketType ticketType);
  Task<TicketType?> GetTicketTypeByChannelIdAsync(ulong channelId);
  Task UpdateTicketTypeAsync(TicketType ticketType);
}