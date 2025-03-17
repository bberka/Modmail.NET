using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Models;

namespace Modmail.NET.Entities;

public sealed class GuildTeamMember
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public ulong Key { get; set; }
  public TeamMemberDataType Type { get; set; }
  public Guid GuildTeamId { get; set; }
  public GuildTeam? GuildTeam { get; set; }

  public static async Task<TeamPermissionLevel?> GetPermissionLevelAsync(ulong userId, List<ulong> roleIdList) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();


    var teamMember = await dbContext.GuildTeamMembers
                                    .Include(x => x.GuildTeam)
                                    .Where(x => (x.Type == TeamMemberDataType.RoleId && roleIdList.Contains(x.Key)) || (x.Key == userId && x.Type == TeamMemberDataType.UserId))
                                    .OrderByDescending(x => x.GuildTeam!.PermissionLevel)
                                    .FirstOrDefaultAsync();
    return teamMember?.GuildTeam!.PermissionLevel;
  }

  public static async Task<List<PermissionInfo>> GetPermissionInfoAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers
                          .Include(x => x.GuildTeam)
                          .Where(x => x.GuildTeam!.IsEnabled)
                          .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
                          .ToListAsync();
  }

  public static async Task<List<PermissionInfo>> GetPermissionInfoOrHigherAsync(TeamPermissionLevel levelOrHigher) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeamMembers
                          .Include(x => x.GuildTeam)
                          .Where(x => x.GuildTeam!.IsEnabled && x.GuildTeam.PermissionLevel >= levelOrHigher)
                          .Select(x => new PermissionInfo(x.GuildTeam!.PermissionLevel, x.Key, x.Type, x.GuildTeam.PingOnNewTicket, x.GuildTeam.PingOnNewMessage))
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