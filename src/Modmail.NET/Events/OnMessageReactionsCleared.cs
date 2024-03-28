using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageReactionsCleared
{
  public static async Task Handle(DiscordClient sender, MessageReactionsClearEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args?.Message?.Author);
  }
}