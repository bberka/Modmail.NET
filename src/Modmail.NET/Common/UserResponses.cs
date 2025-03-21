using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

/// <summary>
///   Contains the messages bot to send to user
/// </summary>
public static class UserResponses
{
  public static DiscordEmbed FeedbackReceivedUpdateMessage(Ticket ticket) {
    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(LangKeys.FEEDBACK_RECEIVED.GetTranslation())
                       .WithCustomTimestamp()
                       .WithGuildInfoFooter()
                       .AddField(LangKeys.STAR.GetTranslation(), LangKeys.STAR_EMOJI.GetTranslation() + ticket.FeedbackStar)
                       .AddField(LangKeys.FEEDBACK.GetTranslation(), ticket.FeedbackMessage)
                       .WithColor(Colors.FeedbackColor);
    return feedbackDone;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_CHANGED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketTypeChangedColor);
    if (!string.IsNullOrEmpty(ticketType?.EmbedMessageTitle) && !string.IsNullOrEmpty(ticketType.EmbedMessageContent))
      embed.AddField(ticketType.EmbedMessageTitle, ticketType.EmbedMessageContent);

    if (ticketType is not null)
      embed.WithDescription(string.Format(LangKeys.TICKET_TYPE_SET.GetTranslation(), ticketType.Emoji, ticketType.Name));
    else
      embed.WithDescription(LangKeys.TICKET_TYPE_REMOVED.GetTranslation());

    return embed;
  }

  public static DiscordEmbedBuilder YourTicketHasBeenClosed(Ticket ticket, GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YOUR_TICKET_HAS_BEEN_CLOSED.GetTranslation())
                .WithDescription(LangKeys.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION.GetTranslation())
                .WithGuildInfoFooter(guildOption)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketClosedColor);

    var closingMessage = LangKeys.CLOSING_MESSAGE_DESCRIPTION.GetTranslation();

    if (!string.IsNullOrEmpty(closingMessage)) embed.WithDescription(closingMessage);

    if (!string.IsNullOrEmpty(ticket.CloseReason)) embed.AddField(LangKeys.CLOSE_REASON.GetTranslation(), ticket.CloseReason);

    return embed;
  }

  public static DiscordMessageBuilder GiveFeedbackMessage(Ticket ticket, GuildOption guildOption) {
    var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
    var starList = new List<DiscordComponent> {
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
    };

    var ticketFeedbackEmbed = new DiscordEmbedBuilder()
                              .WithTitle(LangKeys.FEEDBACK.GetTranslation())
                              .WithDescription(LangKeys.FEEDBACK_DESCRIPTION.GetTranslation())
                              .WithCustomTimestamp()
                              .WithGuildInfoFooter(guildOption)
                              .WithColor(Colors.FeedbackColor);

    var response = ticketFeedbackMsgToUser
                   .AddEmbed(ticketFeedbackEmbed)
                   .AddComponents(starList);
    return response;
  }

  public static DiscordEmbedBuilder TicketPriorityChanged(GuildOption guildOption, DiscordUserInfo info, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithGuildInfoFooter(guildOption)
                .WithTitle(LangKeys.TICKET_PRIORITY_CHANGED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(LangKeys.OLD_PRIORITY.GetTranslation(), oldPriority.ToString(), true)
                .AddField(LangKeys.NEW_PRIORITY.GetTranslation(), newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(info);
    // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }


  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(string reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YOU_HAVE_BEEN_BLACKLISTED.GetTranslation())
                .WithDescription(LangKeys.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION.GetTranslation())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.ErrorColor);

    if (!string.IsNullOrEmpty(reason)) embed.AddField(LangKeys.REASON.GetTranslation(), reason);

    return embed;
  }

  public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
                                                              GuildOption option,
                                                              List<TicketType> ticketTypes,
                                                              Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YOU_HAVE_CREATED_NEW_TICKET.GetTranslation())
                .WithFooter(guild.Name, guild.IconUrl)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor);
    var greetingMessage = LangKeys.GREETING_MESSAGE_DESCRIPTION.GetTranslation();
    if (!string.IsNullOrEmpty(greetingMessage))
      embed.WithDescription(greetingMessage);

    var builder = new DiscordMessageBuilder()
      .AddEmbed(embed);

    if (ticketTypes.Count > 0) {
      var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                 LangKeys.PLEASE_SELECT_A_TICKET_TYPE.GetTranslation(),
                                                 ticketTypes.Select(x => new DiscordSelectComponentOption(x.Name,
                                                                                                          x.Key.ToString(),
                                                                                                          x.Description,
                                                                                                          false,
                                                                                                          !string.IsNullOrWhiteSpace(x.Emoji)
                                                                                                            ? new DiscordComponentEmoji(x.Emoji)
                                                                                                            : null))
                                                            .ToList());
      builder.AddComponents(selectBox);
    }

    return builder;
  }


  public static DiscordEmbedBuilder YouHaveBeenRemovedFromBlacklist(DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST.GetTranslation())
                .WithDescription(LangKeys.YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST_DESCRIPTION.GetTranslation())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithUserAsAuthor(user)
                .WithColor(Colors.SuccessColor);
    return embed;
  }

  public static DiscordEmbedBuilder MessageSent(DiscordMessage message) {
    var embed = new DiscordEmbedBuilder()
                .WithDescription(message.Content)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.MessageSentColor)
                .WithUserAsAuthor(message.Author)
                .AddAttachment(message.Attachments);
    return embed;
  }

  public static DiscordEmbedBuilder MessageReceived(DiscordMessage message, bool anonymous) {
    var embed = new DiscordEmbedBuilder()
                .WithDescription(message.Content)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.MessageReceivedColor)
                .AddAttachment(message.Attachments);
    if (!anonymous) embed.WithUserAsAuthor(message.Author);

    return embed;
  }
}