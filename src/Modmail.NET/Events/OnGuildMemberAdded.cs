using DSharpPlus;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnGuildMemberAdded
{
    public static async Task Handle(DiscordClient sender, GuildMemberAddEventArgs args)
    {
        await DiscordUserInfo.AddOrUpdateAsync(args?.Member);
    }
}