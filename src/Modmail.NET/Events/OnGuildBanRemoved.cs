using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnGuildBanRemoved
{
  public static async Task Handle(DiscordClient sender, GuildBanRemoveEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Member);
  }
}