using DSharpPlus.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static class EmbedMessage
{
  public static DiscordEmbedBuilder MessageSentByMod(DiscordMessage message, bool anonymous) {
    return MessageSent(MessageSentBy.Mod, message, anonymous);
  }

  public static DiscordEmbedBuilder MessageSentByUser(DiscordMessage message) {
    return MessageSent(MessageSentBy.User, message, false);
  }


  private static DiscordEmbedBuilder MessageSent(MessageSentBy messageSentBy, DiscordMessage message, bool anonymous = false) {
    var embed = new DiscordEmbedBuilder()
                .WithGuildInfoFooter()
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(Colors.MessageSentColor)
                .WithUserAsAuthor(message.Author)
                .AddAttachment(message.Attachments);

    if (messageSentBy == MessageSentBy.Mod && anonymous) {
      embed.WithFooter(Texts.THIS_MESSAGE_SENT_ANONYMOUSLY);
    }

    return embed;
  }


  public static DiscordEmbedBuilder MessageReceivedFromMod(DiscordMessage message, bool anonymous) {
    return MessageReceived(MessageSentBy.Mod, message, anonymous);
  }

  public static DiscordEmbedBuilder MessageReceivedFromUser(DiscordMessage message) {
    return MessageReceived(MessageSentBy.User, message, false);
  }

  private static DiscordEmbedBuilder MessageReceived(MessageSentBy messageSentBy, DiscordMessage message, bool anonymous = false) {
    var embed = new DiscordEmbedBuilder()
                .WithGuildInfoFooter()
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(Colors.MessageReceivedColor)
                .AddAttachment(message.Attachments);
    switch (messageSentBy) {
      case MessageSentBy.User:
        embed.WithUserAsAuthor(message.Author);
        break;
      case MessageSentBy.Mod:
        embed.WithUserAsAuthor(anonymous
                                 ? ModmailBot.This.Client.CurrentUser
                                 : message.Author);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(messageSentBy), messageSentBy, null);
    }

    return embed;
  }
}