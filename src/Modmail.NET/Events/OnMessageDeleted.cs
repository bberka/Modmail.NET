using DSharpPlus;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageDeleted
{
    public static async Task Handle(DiscordClient sender, MessageDeleteEventArgs args)
    {
        await DiscordUserInfo.AddOrUpdateAsync(args?.Message?.Author);
    }
}