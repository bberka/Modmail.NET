﻿using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

/// <summary>
///   Contains the embed messages bot to send to log channel
/// </summary>
public static class LogBotMessages
{
  public static DiscordEmbedBuilder NewTicketCreated(DiscordMessage initialMessage,
                                                     DiscordChannel mailChannel,
                                                     Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.NewTicketCreated.GetTranslation())
                .WithUserAsAuthor(initialMessage.Author)
                .WithCustomTimestamp()
                .WithColor(ModmailColors.TicketCreatedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticketId.ToString().ToUpper())
                .AddField(LangKeys.User.GetTranslation(), initialMessage.Author!.Mention);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Database.Entities.Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(LangKeys.TicketClosed.GetTranslation())
                .WithColor(ModmailColors.TicketClosedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OpenedBy.GetTranslation(), ticket.OpenerUser!.GetMention(), true)
                .AddField(LangKeys.ClosedBy.GetTranslation(), ticket.CloserUser!.GetMention(), true)
                .AddField(LangKeys.CloseReason.GetTranslation(), ticket.CloseReason, true)
                .AddField(LangKeys.TicketPriority.GetTranslation(), ticket.Priority.ToString(), true);
    if (ticket.OpenerUser is not null) embed.WithUserAsAuthor(ticket.CloserUser);

    return embed;
  }


  public static DiscordEmbedBuilder BlacklistAdded(DiscordUserInfo author, DiscordUserInfo user, string reason) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.UserBlacklisted.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(ModmailColors.InfoColor)
                .AddField(LangKeys.User.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.UserId.GetTranslation(), user.Id.ToString(), true);


    return embed;
  }

  public static DiscordEmbedBuilder BlacklistRemoved(DiscordUserInfo author, DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.UserBlacklistRemoved.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(ModmailColors.InfoColor)
                .AddField(LangKeys.User.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.UserId.GetTranslation(), user.Id.ToString(), true);
    return embed;
  }


  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Database.Entities.Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TicketPriorityChanged.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(ModmailColors.TicketPriorityChangedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OldPriority.GetTranslation(), oldPriority.ToString(), true)
                .AddField(LangKeys.NewPriority.GetTranslation(), newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(modUser);
    // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }
}