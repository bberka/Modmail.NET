using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public class OnThreadCreated
{
  public static async Task Handle(DiscordClient sender, ThreadCreateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Thread.CurrentMember.Member);
  }
}