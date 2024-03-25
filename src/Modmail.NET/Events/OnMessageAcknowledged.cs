using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageAcknowledged
{
  public static async Task Handle(DiscordClient sender, MessageAcknowledgeEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Message.Author);
  }
}