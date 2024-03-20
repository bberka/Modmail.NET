using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;

namespace Modmail.NET.Events;

public static class InteractionCreated
{
  public static async Task Handle(DiscordClient sender, InteractionCreateEventArgs args) {
   
    await Task.CompletedTask;
  }
}