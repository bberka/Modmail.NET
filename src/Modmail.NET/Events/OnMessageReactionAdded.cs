using DSharpPlus;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageReactionAdded
{
    public static async Task Handle(DiscordClient sender, MessageReactionAddEventArgs args)
    {
        await DiscordUserInfo.AddOrUpdateAsync(args?.User);
    }
}