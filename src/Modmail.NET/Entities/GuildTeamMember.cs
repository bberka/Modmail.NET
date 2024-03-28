using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class GuildTeamMember
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; }

  public ulong Key { get; set; }

  public TeamMemberDataType Type { get; set; }

  public Guid GuildTeamId { get; set; }

  public virtual GuildTeam GuildTeam { get; set; }


  public static async Task<TeamPermissionLevel?> GetPermissionLevelAsync(ulong userId, ulong guildId, List<ulong> roleIdList) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var teamMember = await dbContext.GuildTeamMembers
                                    .Include(x => x.GuildTeam)
                                    .Where(x => x.GuildTeam.GuildOptionId == guildId &&
                                                ((x.Type == TeamMemberDataType.RoleId && roleIdList.Contains(x.Key)) || (x.Key == userId && x.Type == TeamMemberDataType.UserId)))
                                    .OrderByDescending(x => x.GuildTeam.PermissionLevel)
                                    .FirstOrDefaultAsync();
    return teamMember?.GuildTeam.PermissionLevel;
  }

  public static async Task<List<PermissionInfo>> GetPermissionInfoAsync(ulong guildId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers
                          .Include(x => x.GuildTeam)
                          .Where(x => x.GuildTeam.GuildOptionId == guildId && x.GuildTeam.IsEnabled)
                          .Select(x => new PermissionInfo(x.GuildTeam.PermissionLevel, x.Key, x.Type))
                          .ToListAsync();
  }

  public static async Task<List<PermissionInfo>> GetPermissionInfoOrHigherAsync(ulong guildId, TeamPermissionLevel levelOrHigher) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers
                          .Include(x => x.GuildTeam)
                          .Where(x => x.GuildTeam.GuildOptionId == guildId && x.GuildTeam.IsEnabled && x.GuildTeam.PermissionLevel >= levelOrHigher)
                          .Select(x => new PermissionInfo(x.GuildTeam.PermissionLevel, x.Key, x.Type))
                          .ToListAsync();
  }

  public static async Task<bool> IsUserInAnyTeamAsync(ulong memberId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers.AnyAsync(x => x.Key == memberId && x.Type == TeamMemberDataType.UserId);
  }

  public static async Task<bool> IsRoleInAnyTeamAsync(ulong roleId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers.AnyAsync(x => x.Key == roleId && x.Type == TeamMemberDataType.RoleId);
  }
}