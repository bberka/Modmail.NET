using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

/// <summary>
/// Contains the messages bot to send to user
/// </summary>
public static class UserResponses
{
  public static DiscordEmbed FeedbackReceivedUpdateMessage(Ticket ticket) {
    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(Texts.FEEDBACK_RECEIVED)
                       .WithCustomTimestamp()
                       .WithGuildInfoFooter()
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

  public static DiscordEmbedBuilder YourTicketHasBeenClosed(Ticket ticket, GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOUR_TICKET_HAS_BEEN_CLOSED)
                .WithDescription(Texts.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION)
                .WithGuildInfoFooter(guildOption)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketClosedColor);

    if (!string.IsNullOrEmpty(guildOption.ClosingMessage))
      embed.WithDescription(guildOption.ClosingMessage);

    // if (!ticket.Anonymous) {
    //   embed.WithUserAsAuthor(ticket.CloserUserInfo);
    // }
    // else {
    //   embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    // }

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
                              .WithTitle(Texts.FEEDBACK)
                              .WithDescription(Texts.FEEDBACK_DESCRIPTION)
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
                .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(info);
    else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }


  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(string? reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED)
                .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.ErrorColor);

    if (!string.IsNullOrEmpty(reason)) {
      embed.AddField(Texts.REASON, reason);
    }

    return embed;
  }

  public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
                                                              GuildOption option,
                                                              List<TicketType> ticketTypes,
                                                              Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOU_HAVE_CREATED_NEW_TICKET)
                .WithFooter(guild.Name, guild.IconUrl)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor);
    if (!string.IsNullOrEmpty(option.GreetingMessage))
      embed.WithDescription(option.GreetingMessage);

    var builder = new DiscordMessageBuilder()
      .AddEmbed(embed);

    if (ticketTypes.Count > 0) {
      var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                 Texts.PLEASE_SELECT_A_TICKET_TYPE,
                                                 ticketTypes.Select(x => new DiscordSelectComponentOption(x.Name, x.Key.ToString(), x.Description, false, new DiscordComponentEmoji(x.Emoji)))
                                                            .ToList());
      builder.AddComponents(selectBox);
    }

    return builder;
  }


  public static DiscordEmbedBuilder YouHaveBeenRemovedFromBlacklist(DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST)
                .WithDescription(Texts.YOU_HAVE_BEEN_REMOVED_FROM_BLACKLIST_DESCRIPTION)
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
    if (!anonymous) {
      embed.WithUserAsAuthor(message.Author);
    }

    return embed;
  }
}