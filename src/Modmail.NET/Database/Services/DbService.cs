using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Entities;
using Modmail.NET.Models;
using Modmail.NET.Static;

namespace Modmail.NET.Database.Services;

public class DbService : IDbService
{
  private readonly ModmailDbContext _dbContext;

  public DbService(ModmailDbContext dbContext) {
    _dbContext = dbContext;
    _dbContext.Database.SetCommandTimeout(Const.DB_TIMEOUT);
  }

  public async Task<GuildOption?> GetOptionAsync(ulong guildId) {
    return await _dbContext.GuildOptions.FirstOrDefaultAsync(x => x.GuildId == guildId);
  }

  public async Task<Ticket?> GetActiveTicketAsync(ulong discordUserId) {
    return await _dbContext.Tickets
                           .Include(x => x.GuildOption)
                           .FirstOrDefaultAsync(x => x.DiscordUserInfoId == discordUserId && !x.ClosedDateUtc.HasValue);
  }

  public async Task<Ticket?> GetActiveTicketAsync(Guid ticketId) {
    return await _dbContext.Tickets
                           .Include(x => x.GuildOption)
                           .FirstOrDefaultAsync(x => x.Id == ticketId && !x.ClosedDateUtc.HasValue);
  }

  public async Task<ulong> GetLogChannelIdAsync(ulong guildId) {
    return await _dbContext.GuildOptions.Where(x => x.GuildId == guildId).Select(x => x.LogChannelId).FirstOrDefaultAsync();
  }

  public async Task UpdateGuildOptionAsync(GuildOption option) {
    _dbContext.GuildOptions.Update(option);
    await _dbContext.SaveChangesAsync();
  }

  public async Task AddGuildOptionAsync(GuildOption option) {
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


  public async Task<List<GuildTeam>> GetTeamsAsync(ulong guildId) {
    return await _dbContext.GuildTeams
                           .OrderBy(x => x.Id)
                           .Include(x => x.GuildTeamMembers)
                           .Where(x => x.GuildOptionId == guildId)
                           .ToListAsync();
  }

  public async Task<GuildTeam?> GetTeamByIdAsync(ulong guildId, Guid id) {
    return await _dbContext.GuildTeams
                           .Include(x => x.GuildTeamMembers)
                           .Where(x => x.Id == id)
                           .FirstOrDefaultAsync();
  }

  public async Task<GuildTeam?> GetTeamByNameAsync(ulong guildId, string name) {
    return await _dbContext.GuildTeams
                           .OrderBy(x => x.Id)
                           .Include(x => x.GuildTeamMembers)
                           .Where(x => x.Name == name)
                           .FirstOrDefaultAsync();
  }

  public async Task<GuildTeam?> GetTeamByIndexAsync(ulong guildId, int index) {
    return await _dbContext.GuildTeams
                           .OrderBy(x => x.Id)
                           .Include(x => x.GuildTeamMembers)
                           .Skip(index)
                           .Take(1)
                           .FirstOrDefaultAsync();
  }

  public async Task AddNoteAsync(TicketNote noteEntity) {
    await _dbContext.TicketNotes.AddAsync(noteEntity);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<TeamPermissionLevel?> GetPermissionLevelAsync(ulong userId, ulong guildId, List<ulong> roleIdList) {
    var teamMember = await _dbContext.GuildTeamMembers
                                     .Include(x => x.GuildTeam)
                                     .Where(x => x.GuildTeam.GuildOptionId == guildId &&
                                                 ((x.Type == TeamMemberDataType.RoleId && roleIdList.Contains(x.Key)) || (x.Key == userId && x.Type == TeamMemberDataType.UserId)))
                                     .OrderByDescending(x => x.GuildTeam.PermissionLevel)
                                     .FirstOrDefaultAsync();
    return teamMember?.GuildTeam.PermissionLevel;
  }

  public async Task<List<PermissionInfo>> GetPermissionInfoAsync(ulong guildId) {
    return await _dbContext.GuildTeamMembers
                           .Include(x => x.GuildTeam)
                           .Where(x => x.GuildTeam.GuildOptionId == guildId && x.GuildTeam.IsEnabled)
                           .Select(x => new PermissionInfo(x.GuildTeam.PermissionLevel, x.Key, x.Type))
                           .ToListAsync();
  }

  public Task<List<PermissionInfo>> GetPermissionInfoOrHigherAsync(ulong guildId, TeamPermissionLevel levelOrHigher) {
    return _dbContext.GuildTeamMembers
                     .Include(x => x.GuildTeam)
                     .Where(x => x.GuildTeam.GuildOptionId == guildId && x.GuildTeam.IsEnabled && x.GuildTeam.PermissionLevel >= levelOrHigher)
                     .Select(x => new PermissionInfo(x.GuildTeam.PermissionLevel, x.Key, x.Type))
                     .ToListAsync();
  }

  public async Task UpdateUserInfoAsync(DiscordUserInfo dcUserInfo) {
    var current = await _dbContext.DiscordUserInfos.FindAsync(dcUserInfo.Id);


    if (current is not null) {
      const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
      var lastUpdate = current.UpdateDateUtc ?? current.RegisterDateUtc;
      if (lastUpdate.AddHours(waitHoursAfterUpdate) > DateTime.Now) return;
      dcUserInfo.RegisterDateUtc = current.RegisterDateUtc;
      current.UpdateDateUtc = DateTime.UtcNow;
      current.Username = dcUserInfo.Username;
      current.AvatarUrl = dcUserInfo.AvatarUrl;
      current.BannerUrl = dcUserInfo.BannerUrl;
      current.Email = dcUserInfo.Email;
      current.Locale = dcUserInfo.Locale;
      _dbContext.DiscordUserInfos.Update(current);
    }
    else {
      dcUserInfo.RegisterDateUtc = DateTime.UtcNow;
      await _dbContext.DiscordUserInfos.AddAsync(dcUserInfo);
    }

    await _dbContext.SaveChangesAsync();
  }

  public async Task<bool> GetUserBlacklistStatus(ulong authorId) {
    return await _dbContext.TicketBlacklists.AnyAsync(x => x.DiscordUserInfoId == authorId);
  }

  public async Task AddBlacklistAsync(ulong userId, ulong guildId, string? reason) {
    var blacklist = new TicketBlacklist {
      // Id = Guid.NewGuid(),
      DiscordUserInfoId = userId,
      GuildOptionId = guildId,
      Reason = reason,
      RegisterDateUtc = DateTime.UtcNow
    };
    await _dbContext.TicketBlacklists.AddAsync(blacklist);
    await _dbContext.SaveChangesAsync();
  }

  public async Task RemoveBlacklistAsync(ulong userId) {
    var blacklist = await _dbContext.TicketBlacklists.FirstOrDefaultAsync(x => x.DiscordUserInfoId == userId);
    if (blacklist is not null) {
      _dbContext.TicketBlacklists.Remove(blacklist);
      await _dbContext.SaveChangesAsync();
    }
  }

  public async Task<List<ulong>> GetBlacklistedUsersAsync(ulong guildId) {
    return await _dbContext.TicketBlacklists.Where(x => x.GuildOptionId == guildId).Select(x => x.DiscordUserInfoId).ToListAsync();
  }

  public async Task<Ticket> GetClosedTicketAsync(Guid ticketId) {
    return await _dbContext.Tickets
                           .Include(x => x.GuildOption)
                           .SingleAsync(x => x.Id == ticketId && x.ClosedDateUtc.HasValue);
  }

  public async Task AddFeedbackAsync(Guid ticketId, int starCount, string textInput) {
    var ticket = await _dbContext.Tickets.FindAsync(ticketId);

    if (ticket is null) return;

    var isClosed = ticket.ClosedDateUtc.HasValue;
    if (!isClosed) return;

    ticket.FeedbackStar = starCount;
    ticket.FeedbackMessage = textInput;
    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<bool> AnyServerSetupAsync() {
    return await _dbContext.GuildOptions.AnyAsync();
  }

  public async Task<bool> IsUserInAnyTeamAsync(ulong memberId) {
    return await _dbContext.GuildTeamMembers.AnyAsync(x => x.Key == memberId && x.Type == TeamMemberDataType.UserId);
  }

  public async Task<bool> IsRoleInAnyTeamAsync(ulong roleId) {
    return await _dbContext.GuildTeamMembers.AnyAsync(x => x.Key == roleId && x.Type == TeamMemberDataType.RoleId);
  }

  public async Task AddTicketTypeAsync(TicketType ticketType) {
    await _dbContext.TicketTypes.AddAsync(ticketType);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<TicketType?> GetTicketTypeByIdAsync(Guid id) {
    return await _dbContext.TicketTypes.FindAsync(id);
  }

  public async Task<TicketType?> GetTicketTypeByKeyAsync(string key) {
    return await _dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Key == key);
  }

  public async Task<TicketType?> GetTicketTypeByNameAsync(string name) {
    return await _dbContext.TicketTypes.FirstOrDefaultAsync(x => x.Name == name);
  }

  public async Task<bool> TicketTypeExists(string relatedContent) {
    return await _dbContext.TicketTypes.AnyAsync(x => x.Name == relatedContent && x.Key == relatedContent && x.Description == relatedContent && x.Emoji == relatedContent);
  }

  public async Task<List<TicketType>> GetEnabledTicketTypesAsync() {
    return await _dbContext.TicketTypes.ToListAsync();
  }

  public async Task RemoveTicketTypeAsync(TicketType ticketType) {
    _dbContext.TicketTypes.Remove(ticketType);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<TicketType?> GetTicketTypeByChannelIdAsync(ulong channelId) {
    return await _dbContext.Tickets
                           .Include(x => x.TicketType)
                           .Where(x => x.ModMessageChannelId == channelId)
                           .Select(x => x.TicketType)
                           .FirstOrDefaultAsync();
  }

  public async Task AddTeamAsync(GuildTeam team) {
    await _dbContext.GuildTeams.AddAsync(team);
    await _dbContext.SaveChangesAsync();
  }


  public async Task RemoveTeamAsync(GuildTeam team) {
    _dbContext.GuildTeams.Remove(team);
    await _dbContext.SaveChangesAsync();
  }

  public async Task UpdateTeamAsync(GuildTeam team) {
    _dbContext.GuildTeams.Update(team);
    await _dbContext.SaveChangesAsync();
  }
}