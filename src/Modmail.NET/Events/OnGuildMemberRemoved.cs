using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnGuildMemberRemoved
{
  public static async Task Handle(DiscordClient sender, GuildMemberRemoveEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Member);
  }
}