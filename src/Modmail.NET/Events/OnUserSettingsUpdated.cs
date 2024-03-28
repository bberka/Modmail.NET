using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnUserSettingsUpdated
{
  public static async Task Handle(DiscordClient sender, UserSettingsUpdateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args?.User);
  }
}