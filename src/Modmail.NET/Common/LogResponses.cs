using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;

namespace Modmail.NET.Common;

/// <summary>
///  Contains the embed messages bot to send to log channel
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
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticketId.ToString().ToUpper(), false)
                .AddField(LangKeys.USER.GetTranslation(), initialMessage.Author.Mention, false)
      ;
    return embed;
  }

  public static DiscordEmbed MessageSentByMod(DiscordMessage message,
                                              Guid ticketId,
                                              bool ticketAnonymous) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.MESSAGE_SENT_BY_MOD.GetTranslation())
                .WithUserAsAuthor(message.Author)
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(DiscordColor.Green)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticketId.ToString().ToUpper())
                .AddAttachment(message.Attachments);

    if (ticketAnonymous) embed.AddField(LangKeys.ANONYMOUS.GetTranslation(), LangKeys.THIS_MESSAGE_SENT_ANONYMOUSLY.GetTranslation());

    return embed;
  }


  public static DiscordEmbed MessageSentByUser(DiscordMessage message,
                                               Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.MESSAGE_SENT_BY_USER.GetTranslation())
                .WithUserAsAuthor(message.Author)
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(DiscordColor.Green)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticketId.ToString().ToUpper())
                .AddAttachment(message.Attachments);

    return embed;
  }


  public static DiscordEmbed BlacklistAdded(DiscordUserInfo author, DiscordUserInfo user, string? reason) {
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

  public static DiscordEmbed FeedbackReceived(Ticket ticket) {
    var logEmbed = new DiscordEmbedBuilder()
                   .WithTitle(LangKeys.FEEDBACK_RECEIVED.GetTranslation())
                   .WithDescription(ticket.FeedbackMessage)
                   .WithCustomTimestamp()
                   .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper(), false)
                   .AddField(LangKeys.USER.GetTranslation(), ticket.OpenerUser!.GetMention(), true) //not sure needed
                   .AddField(LangKeys.STAR.GetTranslation(), ticket.FeedbackStar.ToString(), true)
                   .WithColor(Colors.FeedbackColor)
                   .WithUserAsAuthor(ticket.OpenerUser);

    return logEmbed;
  }

  public static DiscordEmbedBuilder NoteAdded(Ticket ticket, TicketNote note, DiscordUserInfo user) {
    var noteAddedColorEmbed = new DiscordEmbedBuilder()
                              .WithColor(Colors.NoteAddedColor)
                              .WithTitle(LangKeys.NOTE_ADDED.GetTranslation())
                              .WithDescription(note.Content)
                              .WithCustomTimestamp()
                              .WithUserAsAuthor(user)
                              .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper());
    return noteAddedColorEmbed;
  }

  public static DiscordEmbedBuilder AnonymousToggled(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(ticket.Anonymous
                             ? LangKeys.ANONYMOUS_MOD_ON.GetTranslation()
                             : LangKeys.ANONYMOUS_MOD_OFF.GetTranslation())
                .WithColor(Colors.AnonymousToggledColor)
                .WithCustomTimestamp()
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper())
                .WithDescription(ticket.Anonymous
                                   ? LangKeys.TICKET_SET_ANONYMOUS_DESCRIPTION.GetTranslation()
                                   : LangKeys.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION.GetTranslation());
    if (ticket.OpenerUser is not null) {
      embed.WithUserAsAuthor(ticket.OpenerUser);
    }

    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(Ticket ticket, TicketType? ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_CHANGED.GetTranslation())
                .WithCustomTimestamp()
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper())
                .WithColor(Colors.TicketTypeChangedColor);
    if (ticket.OpenerUser is not null) {
      embed.WithUserAsAuthor(ticket.OpenerUser);
    }

    if (ticketType is not null) {
      embed.WithDescription(string.Format(LangKeys.TICKET_TYPE_SET.GetTranslation(), ticketType.Emoji, ticketType.Name));
    }
    else {
      embed.WithDescription(LangKeys.TICKET_TYPE_REMOVED.GetTranslation());
    }

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
    else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                // .WithDescription("Ticket has been closed.")
                .WithCustomTimestamp()
                .WithTitle(LangKeys.TICKET_CLOSED.GetTranslation())
                .WithColor(Colors.TicketClosedColor)
                .AddField(LangKeys.TICKET_ID.GetTranslation(), ticket.Id.ToString().ToUpper(), false)
                .AddField(LangKeys.OPENED_BY.GetTranslation(), ticket.OpenerUser!.GetMention(), true)
                .AddField(LangKeys.CLOSED_BY.GetTranslation(), ticket.CloserUser!.GetMention(), true)
                .AddField(LangKeys.CLOSE_REASON.GetTranslation(), ticket.CloseReason, true);
    if (ticket.OpenerUser is not null) {
      embed.WithUserAsAuthor(ticket.CloserUser);
    }

    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeDeleted(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_DELETED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TICKET_TYPE.GetTranslation(), ticketType.Name, true)
                .AddField(LangKeys.EMOJI.GetTranslation(), ticketType.Emoji.GetStringOrNA(), true)
                .AddField(LangKeys.ORDER.GetTranslation(), ticketType.Order.ToString(), true)
                .AddField(LangKeys.DESCRIPTION.GetTranslation(), ticketType.Description.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_TITLE.GetTranslation(), ticketType.EmbedMessageTitle.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_CONTENT.GetTranslation(), ticketType.EmbedMessageContent.GetStringOrNA(), false)
      ;
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeUpdated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_UPDATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TICKET_TYPE.GetTranslation(), ticketType.Name, true)
                .AddField(LangKeys.EMOJI.GetTranslation(), ticketType.Emoji.GetStringOrNA(), true)
                .AddField(LangKeys.ORDER.GetTranslation(), ticketType.Order.ToString(), true)
                .AddField(LangKeys.DESCRIPTION.GetTranslation(), ticketType.Description.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_TITLE.GetTranslation(), ticketType.EmbedMessageTitle.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_CONTENT.GetTranslation(), ticketType.EmbedMessageContent.GetStringOrNA(), false);
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeCreated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TICKET_TYPE_CREATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TICKET_TYPE.GetTranslation(), ticketType.Name, true)
                .AddField(LangKeys.EMOJI.GetTranslation(), ticketType.Emoji.GetStringOrNA(), true)
                .AddField(LangKeys.ORDER.GetTranslation(), ticketType.Order.ToString(), true)
                .AddField(LangKeys.DESCRIPTION.GetTranslation(), ticketType.Description.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_TITLE.GetTranslation(), ticketType.EmbedMessageTitle.GetStringOrNA(), false)
                .AddField(LangKeys.EMBED_MESSAGE_CONTENT.GetTranslation(), ticketType.EmbedMessageContent.GetStringOrNA(), false);

    return embed;
  }

  public static DiscordEmbedBuilder TeamCreated(GuildTeam team) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_CREATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), team.Name, true)
                .AddField(LangKeys.PERMISSION_LEVEL.GetTranslation(), team.PermissionLevel.ToString(), true)
                .AddField(LangKeys.PING_ON_NEW_TICKET.GetTranslation(), team.PingOnNewTicket.ToString())
                .AddField(LangKeys.PING_ON_NEW_MESSAGE.GetTranslation(), team.PingOnNewMessage.ToString());
    return embed;
  }

  public static DiscordEmbedBuilder TeamRemoved(string teamName) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_REMOVED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), teamName, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamMemberAdded(DiscordUserInfo userInfo, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_MEMBER_ADDED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.USER.GetTranslation(), userInfo.GetMention(), true)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamMemberRemoved(DiscordUserInfo userInfo, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_MEMBER_REMOVED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.USER.GetTranslation(), userInfo.GetMention(), true)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRoleAdded(DiscordRole role, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_ROLE_ADDED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.ROLE.GetTranslation(), role.Mention, true)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRoleRemoved(ulong roleId, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_ROLE_REMOVED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.ROLE.GetTranslation(), $"<@&{roleId}>", true)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRenamed(string oldName, string newName) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_RENAMED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.OLD_NAME.GetTranslation(), oldName, true)
                .AddField(LangKeys.NEW_NAME.GetTranslation(), newName, true);
    return embed;
  }

  public static DiscordEmbedBuilder SetupComplete(GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.SETUP_COMPLETE.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.GUILD_ID.GetTranslation(), guildOption.GuildId.ToString(), false)
                .AddField(LangKeys.GUILD_NAME.GetTranslation(), guildOption.Name, true)
                .AddField(LangKeys.CATEGORY_ID.GetTranslation(), guildOption.CategoryId.ToString(), true)
                .AddField(LangKeys.LOG_CHANNEL_ID.GetTranslation(), guildOption.LogChannelId.ToString(), true)
                .AddField(LangKeys.SENSITIVE_LOGGING.GetTranslation(), guildOption.IsSensitiveLogging.ToString(), true)
                .AddField(LangKeys.TAKE_FEEDBACK_AFTER_CLOSING.GetTranslation(), guildOption.TakeFeedbackAfterClosing.ToString(), true)
                .AddField(LangKeys.TICKET_TIMEOUT_HOURS.GetTranslation(), guildOption.TicketTimeoutHours.ToString(), false);
    return embed;
  }

  public static DiscordEmbedBuilder ConfigurationUpdated(GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.CONFIGURATION_UPDATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.GUILD_ID.GetTranslation(), guildOption.GuildId.ToString(), false)
                .AddField(LangKeys.GUILD_NAME.GetTranslation(), guildOption.Name, true)
                .AddField(LangKeys.SENSITIVE_LOGGING.GetTranslation(), guildOption.IsSensitiveLogging.ToString(), true)
                .AddField(LangKeys.TAKE_FEEDBACK_AFTER_CLOSING.GetTranslation(), guildOption.TakeFeedbackAfterClosing.ToString(), true)
                .AddField(LangKeys.TICKET_TIMEOUT_HOURS.GetTranslation(), guildOption.TicketTimeoutHours.ToString(), false);
    // .AddField(LangKeys.CLOSING_MESSAGE.GetTranslation(), guildOption.ClosingMessage, false);
    return embed;
  }

  public static DiscordEmbedBuilder TeamUpdated(TeamPermissionLevel oldPermissionLevel,
                                                bool oldPingOnNewTicket,
                                                bool oldPingOnNewMessage,
                                                bool oldIsEnabled,
                                                TeamPermissionLevel teamPermissionLevel,
                                                bool teamPingOnNewTicket,
                                                bool teamPingOnNewMessage,
                                                bool teamIsEnabled,
                                                string teamName) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(LangKeys.TEAM_UPDATED.GetTranslation())
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(LangKeys.TEAM_NAME.GetTranslation(), teamName, false);
    if (oldPermissionLevel != teamPermissionLevel) embed.AddField(LangKeys.PERMISSION_LEVEL_UPDATED.GetTranslation(), $"{oldPermissionLevel} -> {teamPermissionLevel}", true);

    if (oldPingOnNewTicket != teamPingOnNewTicket) embed.AddField(LangKeys.PING_ON_NEW_TICKET_UPDATED.GetTranslation(), $"{oldPingOnNewTicket} -> {teamPingOnNewTicket}", true);

    if (oldPingOnNewMessage != teamPingOnNewMessage) embed.AddField(LangKeys.PING_ON_NEW_MESSAGE_UPDATED.GetTranslation(), $"{oldPingOnNewMessage} -> {teamPingOnNewMessage}", true);

    if (oldIsEnabled != teamIsEnabled) embed.AddField(LangKeys.IS_ENABLED_UPDATED.GetTranslation(), $"{oldIsEnabled} -> {teamIsEnabled}", true);

    return embed;
  }
}