using DSharpPlus;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnGuildBanAdded
{
    public static async Task Handle(DiscordClient sender, GuildBanAddEventArgs args)
    {
        await DiscordUserInfo.AddOrUpdateAsync(args?.Member);
    }
}