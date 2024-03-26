using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
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

  public static async Task<GuildTeam> GetByNameAsync(ulong guildId, string name) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var result = await dbContext.GuildTeams
                                .FirstOrDefaultAsync(x => x.Name == name);
    if (result is null) throw new TeamNotFoundException();
    return result;
  }

  public static async Task ProcessCreateTeamAsync(ulong guildId, string teamName, TeamPermissionLevel permissionLevel) {
    var guildOption = await GuildOption.GetAsync();
    var exists = await GuildTeam.Exists(guildOption.GuildId, teamName);
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
      PermissionLevel = permissionLevel
    };
    await team.AddAsync();
  }

  private static async Task<bool> Exists(ulong guildId, string teamName) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeams.AnyAsync(x => x.GuildOptionId == guildId && x.Name == teamName);
  }

  public static async Task ProcessRemoveTeamAsync(ulong guildId, string teamName) {
    var team = await GuildTeam.GetByNameAsync(guildId, teamName);
    await team.RemoveAsync();
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
  }

  public async Task ProcessRemoveTeamMember(ulong memberId) {
    var memberEntity = GuildTeamMembers.FirstOrDefault(x => x.Key == memberId);
    if (memberEntity is null) {
      throw new MemberNotFoundInTeamException();
    }

    GuildTeamMembers.Remove(memberEntity);
    await UpdateAsync();
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
  }

  public async Task ProcessRemoveRoleFromTeam(DiscordRole role) {
    var roleEntity = GuildTeamMembers.FirstOrDefault(x => x.Key == role.Id);
    if (roleEntity is null) {
      throw new RoleNotFoundInTeamException();
    }

    GuildTeamMembers.Remove(roleEntity);
    await UpdateAsync();
  }

  public async Task ProcessRenameAsync(string newName) {
    Name = newName;
    await UpdateAsync();
  }
}