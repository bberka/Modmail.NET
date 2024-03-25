using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageUpdated
{
  public static async Task Handle(DiscordClient sender, MessageUpdateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Author);
  }
}