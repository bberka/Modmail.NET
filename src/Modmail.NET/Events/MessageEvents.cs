using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Handlers;

namespace Modmail.NET.Events;

public static class MessageEvents
{
  public static async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args) {
    var isPrivate = args.Channel.IsPrivate;
    if (isPrivate) {
      await MailHandler.HandlePrivateMessage(sender, args.Message, args.Channel, args.Author);
    }
  }

  public static async Task OnMessageDeleted(DiscordClient sender, MessageDeleteEventArgs args) { }

  public static async Task OnMessageUpdated(DiscordClient sender, MessageUpdateEventArgs args) { }
}