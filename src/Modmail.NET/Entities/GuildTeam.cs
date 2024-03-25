using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
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
    return await dbContext.GuildTeams
                          .Where(x => x.GuildOptionId == guildId)
                          .Include(x => x.GuildTeamMembers)
                          .ToListAsync();
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

  public static async Task<GuildTeam?> GetByIdAsync(ulong guildId, Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeams.FirstOrDefaultAsync(x => x.Id == id);
  }

  public static async Task<GuildTeam?> GetByNameAsync(ulong guildId, string name) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeams
                          .FirstOrDefaultAsync(x => x.Name == name);
  }

  public static async Task<GuildTeam?> GetByIndexAsync(ulong guildId, int index) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.GuildTeams
                          .OrderBy(x => x.Id)
                          .Include(x => x.GuildTeamMembers)
                          .Skip(index)
                          .Take(1)
                          .FirstOrDefaultAsync();
  }
}