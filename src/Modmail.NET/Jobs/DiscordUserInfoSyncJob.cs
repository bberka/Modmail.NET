using Hangfire;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Serilog;

namespace Modmail.NET.Jobs;

public sealed class DiscordUserInfoSyncJob : HangfireRecurringJobBase
{
  public DiscordUserInfoSyncJob() : base("DiscordUserInfoSyncJob", Cron.Hourly()) { }


  public override async Task Execute() {
    var guild = await ModmailBot.This.GetMainGuildAsync();
    var members = await guild.GetAllMembersAsync();
    var allDbUsers = await DiscordUserInfo.GetAllAsync();
    var convertedDiscordUsers = members.Select(x => new DiscordUserInfo(x) {
      Username = x.GetUsername()
    }).ToList();

    var updateList = new List<DiscordUserInfo>();
    foreach (var dbUser in allDbUsers) {
      var discordUser = convertedDiscordUsers.FirstOrDefault(x => x.Id == dbUser.Id);
      if (discordUser is null) continue;
      var isAllSame = dbUser.AvatarUrl == discordUser.AvatarUrl &&
                      dbUser.Username == discordUser.Username &&
                      dbUser.BannerUrl == discordUser.BannerUrl &&
                      dbUser.Email == discordUser.Email &&
                      dbUser.Locale == discordUser.Locale;
      if (isAllSame) continue;
      dbUser.AvatarUrl = discordUser.AvatarUrl;
      dbUser.Username = discordUser.Username;
      dbUser.BannerUrl = discordUser.BannerUrl;
      dbUser.Email = discordUser.Email;
      dbUser.Locale = discordUser.Locale;
      dbUser.UpdateDateUtc = DateTime.UtcNow;
      updateList.Add(dbUser);
    }

    var dbContext = new ModmailDbContext();
    var addedUsers = convertedDiscordUsers.Where(x => allDbUsers.All(y => y.Id != x.Id)).ToList();
    dbContext.AddRange(addedUsers);
    dbContext.UpdateRange(updateList);
    var affected = await dbContext.SaveChangesAsync();
    Log.Information("DiscordUserInfo table synced with discord guild {Affected}", affected);
  }
}