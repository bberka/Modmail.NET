using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Modmail.NET.Events;

public static class InteractionCreated
{
  public static async Task Handle(DiscordClient sender, InteractionCreateEventArgs args) {
    await Task.CompletedTask;
  }
}