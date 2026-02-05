using DSharpPlus;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnScheduledGuildEventUserAdded
{
    public static async Task Handle(DiscordClient sender, ScheduledGuildEventUserAddEventArgs args)
    {
        await DiscordUserInfo.AddOrUpdateAsync(args?.User);
    }
}