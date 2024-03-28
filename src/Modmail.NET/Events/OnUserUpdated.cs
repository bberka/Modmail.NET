using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnUserUpdated
{
  public static async Task Handle(DiscordClient sender, UserUpdateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.UserAfter);
  }
}