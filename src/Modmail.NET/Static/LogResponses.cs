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
                .WithTitle(LangKeys.NEW_TICKET_CREATED.GetTranslation())
                .WithUserAsAuthor(initialMessage.Author)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticketId.ToString().ToUpper())
                .AddField(LangKeys.USER.GetTranslation(), initialMessage.Author!.Mention);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithCustomTimestamp()
                .WithTitle(LangKeys.TICKET_CLOSED.GetTranslation())
                .WithColor(Colors.TicketClosedColor)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OPENED_BY.GetTranslation(), ticket.OpenerUser!.GetMention(), true)
                .AddField(LangKeys.CLOSED_BY.GetTranslation(), ticket.CloserUser!.GetMention(), true)
                .AddField(LangKeys.CLOSE_REASON.GetTranslation(), ticket.CloseReason, true)
                .AddField(LangKeys.TICKET_PRIORITY.GetTranslation(), ticket.Priority.ToString(), true);
    if (ticket.OpenerUser is not null) embed.WithUserAsAuthor(ticket.CloserUser);

    return embed;
  }


  public static DiscordEmbed BlacklistAdded(DiscordUserInfo author, DiscordUserInfo user, string reason) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.USER_BLACKLISTED.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.USER.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.USER_ID.GetTranslation(), user.Id.ToString(), true);


    return embed;
  }

  public static DiscordEmbed BlacklistRemoved(DiscordUserInfo author, DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.USER_BLACKLIST_REMOVED.GetTranslation())
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.USER.GetTranslation(), user.GetMention(), true)
                .AddField(LangKeys.USER_ID.GetTranslation(), user.Id.ToString(), true);
    return embed;
  }


  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_PRIORITY_CHANGED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper())
                .AddField(LangKeys.OLD_PRIORITY.GetTranslation(), oldPriority.ToString(), true)
                .AddField(LangKeys.NEW_PRIORITY.GetTranslation(), newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(modUser);
    // else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }


  public static DiscordEmbedBuilder TicketTypeUpdated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_UPDATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TICKET_TYPE.GetTranslation(), ticketType.Name, true)
                .AddField(LangKeys.EMOJI.GetTranslation(), ticketType.Emoji.GetStringOrNaN(), true)
                .AddField(LangKeys.ORDER.GetTranslation(), ticketType.Order.ToString(), true)
                .AddField(LangKeys.DESCRIPTION.GetTranslation(), ticketType.Description.GetStringOrNaN())
                .AddField(LangKeys.EMBED_MESSAGE_TITLE.GetTranslation(), ticketType.EmbedMessageTitle.GetStringOrNaN())
                .AddField(LangKeys.EMBED_MESSAGE_CONTENT.GetTranslation(), ticketType.EmbedMessageContent.GetStringOrNaN());
    return embed;
  }


  public static DiscordEmbedBuilder SetupComplete(GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.SETUP_COMPLETE.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.GUILD_ID.GetTranslation(), guildOption.GuildId.ToString())
                .AddField(LangKeys.GUILD_NAME.GetTranslation(), guildOption.Name, true)
                .AddField(LangKeys.CATEGORY_ID.GetTranslation(), guildOption.CategoryId.ToString(), true)
                .AddField(LangKeys.LOG_CHANNEL_ID.GetTranslation(), guildOption.LogChannelId.ToString(), true)
                .AddField(LangKeys.TAKE_FEEDBACK_AFTER_CLOSING.GetTranslation(), guildOption.TakeFeedbackAfterClosing.ToString(), true)
                .AddField(LangKeys.TICKET_TIMEOUT_HOURS.GetTranslation(), guildOption.TicketTimeoutHours.ToString());
    return embed;
  }
}