using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;

namespace Modmail.NET.Entities;

public sealed class DiscordUserInfo
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
  public ulong Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  public DateTime? UpdateDateUtc { get; set; }

  [MaxLength(DbLength.NAME)]
  public required string Username { get; set; }

  [MaxLength(DbLength.URL)]
  public string? AvatarUrl { get; set; }

  [MaxLength(DbLength.URL)]
  public string? BannerUrl { get; set; }

  [MaxLength(DbLength.EMAIL)]
  public string? Email { get; set; }

  [MaxLength(DbLength.LOCALE)]
  public string? Locale { get; set; }

  public List<Ticket> OpenedTickets { get; set; } = [];
  public List<Ticket> ClosedTickets { get; set; } = [];
  public List<Ticket> AssignedTickets { get; set; } = [];

  public string GetMention() {
    return $"<@{Id}>";
  }

  public static async Task<DiscordUserInfo> GetAsync(ulong userId) {
    var key = SimpleCacher.CreateKey(nameof(DiscordUserInfo), nameof(GetAsync), userId);
    return await SimpleCacher.Instance.GetOrSetAsync(key, _get, TimeSpan.FromSeconds(10)) ?? await _get();

    async Task<DiscordUserInfo> _get() {
      if (userId == 0) throw new InvalidUserIdException();
      await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
      var result = await dbContext.DiscordUserInfos.FirstOrDefaultAsync(x => x.Id == userId);
      if (result is not null)
        return result;
      var discordUser = await ModmailBot.This.Client.GetUserAsync(userId);

      if (discordUser is not null) {
        result = new DiscordUserInfo(discordUser) {
          Username = discordUser.GetUsername(),
        };
        await result.AddOrUpdateAsync();
        return result;
      }

      throw new NotFoundWithException(LangKeys.USER, userId);
    }
  }

  public async Task RemoveAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.DiscordUserInfos.Remove(this);
    await dbContext.SaveChangesAsync();
  }

  public static async Task AddOrUpdateAsync(DiscordUser? user) {
    if (user is null) return;
    await new DiscordUserInfo(user) {
      Username = user.GetUsername(),
    }.AddOrUpdateAsync();
  }

  private async Task AddOrUpdateAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();

    var dbData = await dbContext.DiscordUserInfos.FindAsync(Id);
    if (dbData is not null) {
      const int waitHoursAfterUpdate = 24; //updates user information every 24 hours
      var lastUpdate = dbData.UpdateDateUtc ?? dbData.RegisterDateUtc;
      if (lastUpdate.AddHours(waitHoursAfterUpdate) > DateTime.Now) return;
      RegisterDateUtc = dbData.RegisterDateUtc;
      dbData.UpdateDateUtc = DateTime.UtcNow;
      dbData.Username = Username;
      dbData.AvatarUrl = AvatarUrl;
      dbData.BannerUrl = BannerUrl;
      dbData.Email = Email;
      dbData.Locale = Locale;
      dbContext.DiscordUserInfos.Update(dbData);
    }
    else {
      RegisterDateUtc = DateTime.UtcNow;
      await dbContext.DiscordUserInfos.AddAsync(this);
    }

    await dbContext.SaveChangesAsync();
  }

  public static async Task<List<DiscordUserInfo>> GetAllAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.DiscordUserInfos.ToListAsync();
  }
  
  public static async Task<Dictionary<ulong,DiscordUserInfo>> GetAllDictionaryAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return (await dbContext.DiscordUserInfos.ToListAsync()).ToDictionary(x => x.Id, x => x);
  }
}