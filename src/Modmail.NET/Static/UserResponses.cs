﻿using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Utils;

namespace Modmail.NET.Static;

/// <summary>
///   Contains the messages bot to send to user
/// </summary>
public static class UserResponses
{
  public static DiscordEmbed FeedbackReceivedUpdateMessage(Ticket ticket) {
    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(LangKeys.FeedbackReceived.GetTranslation())
                       .WithCustomTimestamp()
                       .WithGuildInfoFooter()
                       .AddField(LangKeys.Star.GetTranslation(), LangKeys.StarEmoji.GetTranslation() + ticket.FeedbackStar)
                       .AddField(LangKeys.Feedback.GetTranslation(), ticket.FeedbackMessage)
                       .WithColor(Colors.FeedbackColor);
    return feedbackDone;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TicketTypeChanged.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketTypeChangedColor);
    if (!string.IsNullOrEmpty(ticketType?.EmbedMessageTitle) && !string.IsNullOrEmpty(ticketType.EmbedMessageContent))
      embed.AddField(ticketType.EmbedMessageTitle, ticketType.EmbedMessageContent);

    if (ticketType is not null)
      embed.WithDescription(string.Format(LangKeys.TicketTypeSet.GetTranslation(), ticketType.Emoji, ticketType.Name));
    else
      embed.WithDescription(LangKeys.TicketTypeRemoved.GetTranslation());

    return embed;
  }

  public static DiscordEmbedBuilder YourTicketHasBeenClosed(Ticket ticket, GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YourTicketHasBeenClosed.GetTranslation())
                .WithDescription(LangKeys.YourTicketHasBeenClosedDescription.GetTranslation())
                .WithGuildInfoFooter(guildOption)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketClosedColor);

    var closingMessage = LangKeys.ClosingMessageDescription.GetTranslation();

    if (!string.IsNullOrEmpty(closingMessage)) embed.WithDescription(closingMessage);

    if (!string.IsNullOrEmpty(ticket.CloseReason)) embed.AddField(LangKeys.CloseReason.GetTranslation(), ticket.CloseReason);

    return embed;
  }

  public static DiscordMessageBuilder GiveFeedbackMessage(Ticket ticket, GuildOption guildOption) {
    var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
    var starList = new List<DiscordComponent> {
      new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
      new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
    };

    var ticketFeedbackEmbed = new DiscordEmbedBuilder()
                              .WithTitle(LangKeys.Feedback.GetTranslation())
                              .WithDescription(LangKeys.FeedbackDescription.GetTranslation())
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
                .WithTitle(LangKeys.TicketPriorityChanged.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(LangKeys.OldPriority.GetTranslation(), oldPriority.ToString(), true)
                .AddField(LangKeys.NewPriority.GetTranslation(), newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(info);
    // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }


  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(string reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YouHaveBeenBlacklisted.GetTranslation())
                .WithDescription(LangKeys.YouHaveBeenBlacklistedDescription.GetTranslation())
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.ErrorColor);

    if (!string.IsNullOrEmpty(reason)) embed.AddField(LangKeys.Reason.GetTranslation(), reason);

    return embed;
  }

  public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
                                                              GuildOption option,
                                                              List<TicketType> ticketTypes,
                                                              Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.YouHaveCreatedNewTicket.GetTranslation())
                .WithFooter(guild.Name, guild.IconUrl)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor);
    var greetingMessage = LangKeys.GreetingMessageDescription.GetTranslation();
    if (!string.IsNullOrEmpty(greetingMessage))
      embed.WithDescription(greetingMessage);

    var builder = new DiscordMessageBuilder()
      .AddEmbed(embed);

    if (ticketTypes.Count > 0) {
      var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                 LangKeys.PleaseSelectATicketType.GetTranslation(),
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
                .WithTitle(LangKeys.YouHaveBeenRemovedFromBlacklist.GetTranslation())
                .WithDescription(LangKeys.YouHaveBeenRemovedFromBlacklistDescription.GetTranslation())
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