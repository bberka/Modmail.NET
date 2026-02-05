using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Serilog;

namespace Modmail.NET.Manager;

public sealed class DiscordUserInfoSyncTimer
{
    private static DiscordUserInfoSyncTimer? _instance;

    private readonly Timer _timer;

    private DiscordUserInfoSyncTimer()
    {
        const int intervalSeconds = 60 * 60; // 1 hour
        _timer = new Timer(TimerElapsed, null, 0, intervalSeconds * 1000);
        Log.Information("{ServiceName} initialized", nameof(DiscordUserInfoSyncTimer));
    }

    public static DiscordUserInfoSyncTimer This
    {
        get
        {
            _instance ??= new DiscordUserInfoSyncTimer();
            return _instance;
        }
    }

    private void TimerElapsed(object? sender)
    {
        TimerElapsedAsync()
            .GetAwaiter()
            .GetResult();
    }

    private async Task TimerElapsedAsync()
    {
        try
        {
            var guild = await ModmailBot.This.GetMainGuildAsync();
            var members = await guild.GetAllMembersAsync();
            var allDbUsers = await DiscordUserInfo.GetAllAsync();
            var convertedDiscordUsers = members.Select(x => new DiscordUserInfo(x)
                {
                    Username = x.GetUsername()
                })
                .ToList();

            var updateList = new List<DiscordUserInfo>();
            foreach (var dbUser in allDbUsers)
            {
                var discordUser = convertedDiscordUsers.FirstOrDefault(x => x.Id == dbUser.Id);
                if (discordUser is null) continue;
                var isAllSame = dbUser.AvatarUrl == discordUser.AvatarUrl && dbUser.Username == discordUser.Username &&
                                dbUser.BannerUrl == discordUser.BannerUrl && dbUser.Email == discordUser.Email && dbUser.Locale == discordUser.Locale;
                if (isAllSame) continue;
                dbUser.AvatarUrl = discordUser.AvatarUrl;
                dbUser.Username = discordUser.Username;
                dbUser.BannerUrl = discordUser.BannerUrl;
                dbUser.Email = discordUser.Email;
                dbUser.Locale = discordUser.Locale;
                dbUser.UpdateDateUtc = DateTime.UtcNow;
                updateList.Add(dbUser);
            }

            var dbContext = ServiceLocator.Get<ModmailDbContext>();
            var addedUsers = convertedDiscordUsers.Where(x => allDbUsers.All(y => y.Id != x.Id))
                .ToList();
            dbContext.AddRange(addedUsers);
            dbContext.UpdateRange(updateList);
            var affected = await dbContext.SaveChangesAsync();
            Log.Information("DiscordUserInfo table synced with discord guild {affected}", affected);
        }
        catch (BotExceptionBase ex)
        {
            Log.Warning(ex, "DiscordUserInfo table synced with discord guild");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "DiscordUserInfo table synced with discord guild");
        }
    }
}