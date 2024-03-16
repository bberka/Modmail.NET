using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Modmail.NET.Events;

public static class InteractionEventHandlers
{
  public static async Task InteractionCreated(DiscordClient sender, InteractionCreateEventArgs args) {
    await Task.CompletedTask;
  }

  public static async Task ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    await Task.CompletedTask;
  }
}