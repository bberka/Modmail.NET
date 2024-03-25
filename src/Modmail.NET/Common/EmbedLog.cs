using DSharpPlus.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;

namespace Modmail.NET.Common;

public static
  class EmbedLog
{
  public static DiscordEmbedBuilder NewTicketCreated(DiscordMessage initialMessage,
                                                     DiscordChannel mailChannel,
                                                     Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.NEW_TICKET_CREATED)
                .WithUserAsAuthor(initialMessage.Author)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor)
                .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), false)
                .AddField(Texts.USER, initialMessage.Author.Mention, false)
      ;
    return embed;
  }

  public static DiscordEmbed MessageSentByMod(DiscordMessage message,
                                              Guid ticketId,
                                              bool ticketAnonymous) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.MESSAGE_SENT_BY_MOD)
                .WithUserAsAuthor(message.Author)
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(DiscordColor.Green)
                .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper())
                .AddAttachment(message.Attachments);

    if (ticketAnonymous) embed.AddField(Texts.ANONYMOUS, Texts.THIS_MESSAGE_SENT_ANONYMOUSLY);

    return embed;
  }


  public static DiscordEmbed MessageSentByUser(DiscordMessage message,
                                               Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.MESSAGE_SENT_BY_USER)
                .WithUserAsAuthor(message.Author)
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(DiscordColor.Green)
                .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper())
                .AddAttachment(message.Attachments);

    return embed;
  }


  public static DiscordEmbed BlacklistAdded(DiscordUser author, DiscordUser user, string? reason) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.USER_BLACKLISTED)
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, user.Mention, true)
                .AddField(Texts.USER_ID, user.Id.ToString(), true)
                .AddField(Texts.USERNAME, user.GetUsername(), true);

    return embed;
  }

  public static DiscordEmbed BlacklistRemoved(DiscordUser author, DiscordUser user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.USER_BLACKLIST_REMOVED)
                .WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl)
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, user.Mention, true)
                .AddField(Texts.USER_ID, user.Id.ToString(), true)
                .AddField(Texts.USERNAME, user.GetUsername(), true);
    return embed;
  }
}