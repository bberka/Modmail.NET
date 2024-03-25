using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnMessageReactionRemovedEmoji
{
  public static async Task Handle(DiscordClient sender, MessageReactionRemoveEmojiEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Message.Author);
  }
}