using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;

namespace Modmail.NET.Static;

/// <summary>
///   Contains the embed messages bot to send to log channel
/// </summary>
public static class LogResponses
{
  public static DiscordEmbedBuilder NewTicketCreated(DiscordMessage initialMessage,
                                                     DiscordChannel mailChannel,
                                                     Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.NewTicketCreated.GetTranslation())
                .WithUserAsAuthor(initialMessage.Author)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticketId.ToString().ToUpper())
                .AddField(LangKeys.User.GetTranslation(), initialMessage.Author!.Mention);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(LangKeys.TicketClosed.GetTranslation())
                .WithColor(Colors.TicketClosedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OpenedBy.GetTranslation(), ticket.OpenerUser!.GetMention(), true)
                .AddField(LangKeys.ClosedBy.GetTranslation(), ticket.CloserUser!.GetMention(), true)
                .AddField(LangKeys.CloseReason.GetTranslation(), ticket.CloseReason, true)
                .AddField(LangKeys.TicketPriority.GetTranslation(), ticket.Priority.ToString(), true);
    if (ticket.OpenerUser is not null) embed.WithUserAsAuthor(ticket.CloserUser);

    return embed;
  }


  public static DiscordEmbed BlacklistAdded(DiscordUserInfo author, DiscordUserInfo user, string reason) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.UserBlacklisted.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.User.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.UserId.GetTranslation(), user.Id.ToString(), true);


    return embed;
  }

  public static DiscordEmbed BlacklistRemoved(DiscordUserInfo author, DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.UserBlacklistRemoved.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.User.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.UserId.GetTranslation(), user.Id.ToString(), true);
    return embed;
  }


  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TicketPriorityChanged.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(LangKeys.TicketId.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OldPriority.GetTranslation(), oldPriority.ToString(), true)
                .AddField(LangKeys.NewPriority.GetTranslation(), newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(modUser);
    // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }


  public static DiscordEmbedBuilder TicketTypeUpdated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TicketTypeUpdated.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TicketType.GetTranslation(), ticketType.Name, true)
                .AddField(LangKeys.Emoji.GetTranslation(), ticketType.Emoji.GetStringOrNaN(), true)
                .AddField(LangKeys.Order.GetTranslation(), ticketType.Order.ToString(), true)
                .AddField(LangKeys.Description.GetTranslation(), ticketType.Description.GetStringOrNaN())
                .AddField(LangKeys.EmbedMessageTitle.GetTranslation(), ticketType.EmbedMessageTitle.GetStringOrNaN())
                .AddField(LangKeys.EmbedMessageContent.GetTranslation(), ticketType.EmbedMessageContent.GetStringOrNaN());
    return embed;
  }


  public static DiscordEmbedBuilder SetupComplete(GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.SetupComplete.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.GuildId.GetTranslation(), guildOption.GuildId.ToString())
                .AddField(LangKeys.GuildName.GetTranslation(), guildOption.Name, true)
                .AddField(LangKeys.CategoryId.GetTranslation(), guildOption.CategoryId.ToString(), true)
                .AddField(LangKeys.LogChannelId.GetTranslation(), guildOption.LogChannelId.ToString(), true)
                .AddField(LangKeys.TakeFeedbackAfterClosing.GetTranslation(), guildOption.TakeFeedbackAfterClosing.ToString(), true)
                .AddField(LangKeys.TicketTimeoutHours.GetTranslation(), guildOption.TicketTimeoutHours.ToString());
    return embed;
  }
}