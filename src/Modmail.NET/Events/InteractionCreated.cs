using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;

namespace Modmail.NET.Events;

public static class InteractionCreated
{
  public static async Task Handle(DiscordClient sender, InteractionCreateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Interaction.User);
    await Task.CompletedTask;
  }
}