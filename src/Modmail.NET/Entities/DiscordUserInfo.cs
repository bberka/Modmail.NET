using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Entities;

public class DiscordUserInfo
{
  public DiscordUserInfo() { }

  public DiscordUserInfo(DiscordUser user) {
    Id = user.Id;
    Username = user.GetUsername();
    AvatarUrl = user.AvatarUrl;
    BannerUrl = user.BannerUrl;
    Email = user.Email;
    Locale = user.Locale;
  }

  public DiscordUserInfo(DiscordMember member) {
    Id = member.Id;
    Username = member.GetUsername();
    AvatarUrl = member.AvatarUrl;
    BannerUrl = member.BannerUrl;
    Email = member.Email;
    Locale = member.Locale;
  }

  /// <summary>
  ///   Users Discord Id
  /// </summary>
  [Key]
  public ulong Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  public DateTime? UpdateDateUtc { get; set; }

  public string Username { get; set; }

  public string? AvatarUrl { get; set; }

  public string? BannerUrl { get; set; }

  public string? Email { get; set; }

  public string? Locale { get; set; }

  public virtual List<TicketBlacklist> TicketBlacklists { get; set; }

  public string GetMention() => $"<@{Id}>";

  public static async Task<DiscordUserInfo?> GetAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.DiscordUserInfos.FirstOrDefaultAsync(x => x.Id == userId);
  }

  public async Task RemoveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.DiscordUserInfos.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task AddOrUpdateAsync(DiscordUser user) {
    await new DiscordUserInfo(user).AddOrUpdateAsync();
  }

  private async Task AddOrUpdateAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();

    var dbData = await dbContext.DiscordUserInfos.FindAsync(this.Id);
    if (dbData is not null) {
      const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
      var lastUpdate = dbData.UpdateDateUtc ?? dbData.RegisterDateUtc;
      if (lastUpdate.AddHours(waitHoursAfterUpdate) > DateTime.Now) return;
      this.RegisterDateUtc = dbData.RegisterDateUtc;
      dbData.UpdateDateUtc = DateTime.UtcNow;
      dbData.Username = this.Username;
      dbData.AvatarUrl = this.AvatarUrl;
      dbData.BannerUrl = this.BannerUrl;
      dbData.Email = this.Email;
      dbData.Locale = this.Locale;
      dbContext.DiscordUserInfos.Update(dbData);
    }
    else {
      this.RegisterDateUtc = DateTime.UtcNow;
      await dbContext.DiscordUserInfos.AddAsync(this);
    }

    await dbContext.SaveChangesAsync();
  }

  public static async Task<List<DiscordUserInfo>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.DiscordUserInfos.ToListAsync();
  }
}