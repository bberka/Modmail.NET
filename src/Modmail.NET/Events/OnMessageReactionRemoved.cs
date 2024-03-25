using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageReactionRemoved
{
  public static async Task Handle(DiscordClient sender, MessageReactionRemoveEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.User);
  }
}