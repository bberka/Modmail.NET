using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    await Task.CompletedTask;
  }
}