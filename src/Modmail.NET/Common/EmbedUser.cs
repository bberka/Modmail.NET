using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

public static class EmbedUser
{
  public static DiscordEmbed FeedbackReceivedUpdateMessage(Ticket ticket) {
    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(Texts.FEEDBACK_RECEIVED)
                       .WithCustomTimestamp()
                       .WithGuildInfoFooter(ticket.GuildOption)
                       .AddField(Texts.STAR, Texts.STAR_EMOJI + ticket.FeedbackStar)
                       .AddField(Texts.FEEDBACK, ticket.FeedbackMessage)
                       .WithColor(Colors.FeedbackColor);
    return feedbackDone;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_CHANGED)
                .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                .WithCustomTimestamp()
                .WithColor(Colors.TicketTypeChangedColor);
    if (!string.IsNullOrEmpty(ticketType.EmbedMessageTitle) && !string.IsNullOrEmpty(ticketType.EmbedMessageContent))
      embed.AddField(ticketType.EmbedMessageTitle, ticketType.EmbedMessageContent);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOUR_TICKET_HAS_BEEN_CLOSED)
                .WithDescription(Texts.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION)
                .WithGuildInfoFooter(ticket.GuildOption)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketClosedColor);
    if (!string.IsNullOrEmpty(ticket.GuildOption.ClosingMessage))
      embed.WithDescription(ticket.GuildOption.ClosingMessage);

    if (!ticket.Anonymous) {
      embed.WithUserAsAuthor(ticket.CloserUserInfo);
    }
    else {
      embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    }

    return embed;
  }

  public static DiscordMessageBuilder GiveFeedbackMessage(Ticket ticket) {
    var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
    var starList = new List<DiscordComponent> {
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
    };

    var ticketFeedbackEmbed = new DiscordEmbedBuilder()
                              .WithTitle(Texts.FEEDBACK)
                              .WithDescription(Texts.FEEDBACK_DESCRIPTION)
                              .WithCustomTimestamp()
                              .WithGuildInfoFooter(ticket.GuildOption)
                              .WithColor(Colors.FeedbackColor);

    var response = ticketFeedbackMsgToUser
                   .AddEmbed(ticketFeedbackEmbed)
                   .AddComponents(starList);
    return response;
  }

  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo info, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithGuildInfoFooter(ticket.GuildOption)
                .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(info);
    else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }
}