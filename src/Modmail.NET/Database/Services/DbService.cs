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

  // public async Task<List<Tag>> GetTagsAsync(ulong guildId) {
  //   return await _dbContext.Tags.Where(x => x.GuildOptionId == guildId).ToListAsync();
  // }
  //
  // public async Task<Tag?> GetTagAsync(ulong guildId, string key) {
  //   return await _dbContext.Tags.FirstOrDefaultAsync(x => x.GuildOptionId == guildId && x.Key == key);
  // }
  //
  // public async Task AddTagAsync(Tag tag) {
  //   await _dbContext.Tags.AddAsync(tag);
  //   await _dbContext.SaveChangesAsync();
  // }
  //
  // public async Task RemoveTagAsync(Tag tag) {
  //   _dbContext.Tags.Remove(tag);
  //   await _dbContext.SaveChangesAsync();
  // }

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
      if (lastUpdate.AddHours(waitHoursAfterUpdate) > DateTime.Now) {
        return;
      }
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
      RegisterDateUtc = DateTime.UtcNow,
      
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