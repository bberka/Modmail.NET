using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnScheduledGuildEventUserRemoved
{
  public static async Task Handle(DiscordClient sender, ScheduledGuildEventUserRemoveEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args?.User);
  }
}