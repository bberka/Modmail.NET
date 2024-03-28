using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class GuildTeam
{
  [Key]
  public Guid Id { get; set; }

  public TeamPermissionLevel PermissionLevel { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
  public string Name { get; set; }
  public bool IsEnabled { get; set; } = true;
  public ulong GuildOptionId { get; set; }
  public bool PingOnNewTicket { get; set; }
  public bool PingOnNewMessage { get; set; }

  public virtual GuildOption GuildOption { get; set; }

  public virtual List<GuildTeamMember> GuildTeamMembers { get; set; }

  public static async Task<List<GuildTeam>> GetAllAsync(ulong guildId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.GuildTeams
                                .Where(x => x.GuildOptionId == guildId)
                                .Include(x => x.GuildTeamMembers)
                                .ToListAsync();

    if (result.Count == 0) {
      throw new NoTeamFoundException();
    }

    return result;
  }


  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.GuildTeams.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task RemoveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildTeams.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task UpdateAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildTeams.Update(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task<GuildTeam> GetByNameAsync(string name) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.GuildTeams
                                .FirstOrDefaultAsync(x => x.Name == name);
    if (result is null) throw new TeamNotFoundException();
    return result;
  }

  public static async Task ProcessCreateTeamAsync(ulong guildId,
                                                  string teamName,
                                                  TeamPermissionLevel permissionLevel,
                                                  bool pingOnNewTicket = false,
                                                  bool pingOnTicketMessage = false) {
    var exists = await GuildTeam.Exists(teamName);
    if (exists) {
      throw new TeamAlreadyExistsException();
    }

    var team = new GuildTeam {
      GuildOptionId = guildId,
      Name = teamName,
      RegisterDateUtc = DateTime.UtcNow,
      IsEnabled = true,
      GuildTeamMembers = new List<GuildTeamMember>(),
      UpdateDateUtc = null,
      Id = Guid.NewGuid(),
      PermissionLevel = permissionLevel,
      PingOnNewMessage = pingOnTicketMessage,
      PingOnNewTicket = pingOnNewTicket
    };
    await team.AddAsync();

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamCreated(team));
  }

  private static async Task<bool> Exists(string teamName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeams.AnyAsync(x => x.Name == teamName);
  }

  public async Task ProcessRemoveTeamAsync() {
    await RemoveAsync();
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamRemoved(Name));
  }

  public async Task ProcessAddTeamMemberAsync(ulong memberId) {
    var isUserAlreadyInTeam = await GuildTeamMember.IsUserInAnyTeamAsync(memberId);
    if (isUserAlreadyInTeam) {
      throw new MemberAlreadyInTeamException();
    }

    var memberEntity = new GuildTeamMember {
      GuildTeamId = Id,
      Type = TeamMemberDataType.UserId,
      Key = memberId,
      RegisterDateUtc = DateTime.UtcNow
    };
    GuildTeamMembers.Add(memberEntity);
    await UpdateAsync();


    var userInfo = await DiscordUserInfo.GetAsync(memberId);
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamMemberAdded(userInfo, Name));
  }

  public async Task ProcessRemoveTeamMember(ulong memberId) {
    var memberEntity = GuildTeamMembers.FirstOrDefault(x => x.Key == memberId);
    if (memberEntity is null) {
      throw new MemberNotFoundInTeamException();
    }

    GuildTeamMembers.Remove(memberEntity);
    await UpdateAsync();

    var userInfo = await DiscordUserInfo.GetAsync(memberId);
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamMemberRemoved(userInfo, Name));
  }

  public async Task ProcessAddRoleToTeam(DiscordRole role) {
    var isRoleAlreadyInTeam = await GuildTeamMember.IsRoleInAnyTeamAsync(role.Id);
    if (isRoleAlreadyInTeam) {
      throw new RoleAlreadyInTeamException();
    }

    var roleEntity = new GuildTeamMember {
      GuildTeamId = Id,
      Type = TeamMemberDataType.RoleId,
      Key = role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };
    GuildTeamMembers.Add(roleEntity);
    await UpdateAsync();

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamRoleAdded(role, Name));
  }

  public async Task ProcessRemoveRoleFromTeam(DiscordRole role) {
    var roleEntity = GuildTeamMembers.FirstOrDefault(x => x.Key == role.Id);
    if (roleEntity is null) {
      throw new RoleNotFoundInTeamException();
    }

    GuildTeamMembers.Remove(roleEntity);
    await UpdateAsync();

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamRoleRemoved(role, Name));
  }

  public async Task ProcessRenameAsync(string newName) {
    var oldName = Name;
    Name = newName;
    await UpdateAsync();

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamRenamed(oldName, newName));
  }

  public async Task ProcessUpdateTeamAsync(ulong guildId,
                                           string teamName,
                                           TeamPermissionLevel? permissionLevel,
                                           bool? pingOnNewTicket,
                                           bool? pingOnTicketMessage,
                                           bool? isEnabled) {
    var oldPermissionLevel = PermissionLevel;
    var oldPingOnNewTicket = PingOnNewTicket;
    var oldPingOnNewMessage = PingOnNewMessage;
    var oldIsEnabled = IsEnabled;

    var anyChanges = permissionLevel.HasValue || pingOnNewTicket.HasValue || pingOnTicketMessage.HasValue;

    if (!anyChanges) {
      return;
    }

    var team = await GetByNameAsync(teamName);
    if (permissionLevel.HasValue) {
      team.PermissionLevel = permissionLevel.Value;
    }

    if (pingOnNewTicket.HasValue) {
      team.PingOnNewTicket = pingOnNewTicket.Value;
    }

    if (pingOnTicketMessage.HasValue) {
      team.PingOnNewMessage = pingOnTicketMessage.Value;
    }

    if (isEnabled.HasValue) {
      team.IsEnabled = isEnabled.Value;
    }

    team.UpdateDateUtc = DateTime.UtcNow;
    await team.UpdateAsync();


    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamUpdated(oldPermissionLevel,
                                                               oldPingOnNewTicket,
                                                               oldPingOnNewMessage,
                                                               oldIsEnabled,
                                                               team.PermissionLevel,
                                                               team.PingOnNewTicket,
                                                               team.PingOnNewMessage,
                                                               team.IsEnabled,
                                                               team.Name));
  }
}