using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Features.UserInfo;
using Serilog;

namespace Modmail.NET.Jobs;

public sealed class DiscordUserInfoSyncJob : HangfireRecurringJobBase
{
  private readonly IServiceScopeFactory _scopeFactory;

  public DiscordUserInfoSyncJob(IServiceScopeFactory scopeFactory) : base("DiscordUserInfoSyncJob", Cron.Hourly()) {
    _scopeFactory = scopeFactory;
  }


  public override async Task Execute() {
    var scope = _scopeFactory.CreateScope();
    var bot = scope.ServiceProvider.GetRequiredService<ModmailBot>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var guild = await bot.GetMainGuildAsync();
    var members = await guild.GetAllMembersAsync();
    // var allDbUsers = await DiscordUserInfo.GetAllAsync();
    var allDbUsers = await sender.Send(new GetDiscordUserInfoListQuery());
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

    var addedUsers = convertedDiscordUsers.Where(x => allDbUsers.All(y => y.Id != x.Id)).ToList();
    dbContext.AddRange(addedUsers);
    dbContext.UpdateRange(updateList);
    var affected = await dbContext.SaveChangesAsync();
    Log.Information("DiscordUserInfo table synced with discord guild {Affected}", affected);
  }
}