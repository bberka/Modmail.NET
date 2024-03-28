using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnTypingStarted
{
  public async static Task Handle(DiscordClient sender, TypingStartEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.User);
  }
}