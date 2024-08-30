using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Entities;

public sealed class GuildTeam
{
  public Guid Id { get; set; }
  public TeamPermissionLevel PermissionLevel { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }

  [MaxLength(DbLength.NAME)]
  public required string Name { get; set; }

  public bool IsEnabled { get; set; } = true;
  public bool PingOnNewTicket { get; set; }
  public bool PingOnNewMessage { get; set; }

  //FK
  public List<GuildTeamMember>? GuildTeamMembers { get; set; }

  public static async Task<List<GuildTeam>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.GuildTeams
                                .Include(x => x.GuildTeamMembers)
                                .ToListAsync();

    if (result.Count == 0) throw new EmptyListResultException(LangKeys.TEAM);

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
    if (result is null) throw new NotFoundWithException(LangKeys.TEAM, name);
    return result;
  }

  public static async Task ProcessCreateTeamAsync(string teamName,
                                                  TeamPermissionLevel permissionLevel,
                                                  bool pingOnNewTicket = false,
                                                  bool pingOnTicketMessage = false) {
    var exists = await Exists(teamName);
    if (exists) throw new TeamAlreadyExistsException();

    var team = new GuildTeam {
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
    if (isUserAlreadyInTeam) throw new MemberAlreadyInTeamException();

    var memberEntity = new GuildTeamMember {
      GuildTeamId = Id,
      Type = TeamMemberDataType.UserId,
      Key = memberId,
      RegisterDateUtc = DateTime.UtcNow
    };
    var dbContext = ServiceLocator.Get<ModmailDbContext>();

    dbContext.GuildTeamMembers.Add(memberEntity);
    await dbContext.SaveChangesAsync();

    var userInfo = await DiscordUserInfo.GetAsync(memberId);
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamMemberAdded(userInfo, Name));
  }

  public async Task ProcessRemoveTeamMember(ulong teamMemberKey, TeamMemberDataType type) {
    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var memberEntity = await dbContext.GuildTeamMembers
                                      .FirstOrDefaultAsync(x => x.Key == teamMemberKey && x.Type == type);
    if (memberEntity is null) throw new NotFoundInException(LangKeys.MEMBER, LangKeys.TEAM);
    dbContext.GuildTeamMembers.Remove(memberEntity);
    await dbContext.SaveChangesAsync();
      var logChannel = await ModmailBot.This.GetLogChannelAsync();
    if (type == TeamMemberDataType.UserId) {
      var userInfo = await DiscordUserInfo.GetAsync(teamMemberKey);
      await logChannel.SendMessageAsync(LogResponses.TeamMemberRemoved(userInfo, Name));
    }
    else {
      await logChannel.SendMessageAsync(LogResponses.TeamRoleRemoved(teamMemberKey, Name));
    }
  }

  public async Task ProcessAddRoleToTeam(DiscordRole role) {
    var isRoleAlreadyInTeam = await GuildTeamMember.IsRoleInAnyTeamAsync(role.Id);
    if (isRoleAlreadyInTeam) throw new RoleAlreadyInTeamException();

    var roleEntity = new GuildTeamMember {
      GuildTeamId = Id,
      Type = TeamMemberDataType.RoleId,
      Key = role.Id,
      RegisterDateUtc = DateTime.UtcNow
    };

    var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.GuildTeamMembers.Add(roleEntity);
    await dbContext.SaveChangesAsync();
    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TeamRoleAdded(role, Name));
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

    if (!anyChanges) return;

    var team = await GetByNameAsync(teamName);
    if (permissionLevel.HasValue) team.PermissionLevel = permissionLevel.Value;

    if (pingOnNewTicket.HasValue) team.PingOnNewTicket = pingOnNewTicket.Value;

    if (pingOnTicketMessage.HasValue) team.PingOnNewMessage = pingOnTicketMessage.Value;

    if (isEnabled.HasValue) team.IsEnabled = isEnabled.Value;

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