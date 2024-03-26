using DSharpPlus.Entities;
using Modmail.NET.Entities;
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


  public static DiscordEmbed BlacklistAdded(DiscordUserInfo author, DiscordUserInfo user, string? reason) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.USER_BLACKLISTED)
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, user.GetMention(), true);


    return embed;
  }

  public static DiscordEmbed BlacklistRemoved(DiscordUserInfo author, DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.USER_BLACKLIST_REMOVED)
                .WithUserAsAuthor(author)
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, user.GetMention(), true);
    return embed;
  }

  public static DiscordEmbed FeedbackReceived(Ticket ticket) {
    var logEmbed = new DiscordEmbedBuilder()
                   .WithTitle(Texts.FEEDBACK_RECEIVED)
                   .WithDescription(ticket.FeedbackMessage)
                   .WithCustomTimestamp()
                   .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper(), false)
                   .AddField(Texts.USER, ticket.OpenerUserInfo.GetMention(), true) //not sure needed
                   .AddField(Texts.STAR, ticket.FeedbackStar.ToString(), true)
                   .WithColor(Colors.FeedbackColor)
                   .WithUserAsAuthor(ticket.OpenerUserInfo);

    return logEmbed;
  }

  public static DiscordEmbedBuilder NoteAdded(Ticket ticket, TicketNote note, DiscordUserInfo user) {
    var noteAddedColorEmbed = new DiscordEmbedBuilder()
                              .WithColor(Colors.NoteAddedColor)
                              .WithTitle(Texts.NOTE_ADDED)
                              .WithDescription(note.Content)
                              .WithCustomTimestamp()
                              .WithUserAsAuthor(user)
                              .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper());
    return noteAddedColorEmbed;
  }

  public static DiscordEmbedBuilder AnonymousToggled(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(ticket.Anonymous
                             ? Texts.ANONYMOUS_MOD_ON
                             : Texts.ANONYMOUS_MOD_OFF)
                .WithColor(Colors.AnonymousToggledColor)
                .WithCustomTimestamp()
                .WithUserAsAuthor(ticket.OpenerUserInfo)
                .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                .WithDescription(ticket.Anonymous
                                   ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                                   : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(Ticket ticket, TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_CHANGED)
                .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                .WithUserAsAuthor(ticket.OpenerUserInfo)
                .WithCustomTimestamp()
                .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                .WithColor(Colors.TicketTypeChangedColor);
    return embed;
  }

  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketPriorityChangedColor)
                .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
    if (!ticket.Anonymous) embed.WithUserAsAuthor(modUser);
    else embed.WithUserAsAuthor(ModmailBot.This.Client.CurrentUser);
    return embed;
  }

  public static DiscordEmbedBuilder TicketClosed(Ticket ticket) {
    var embed = new DiscordEmbedBuilder()
                // .WithDescription("Ticket has been closed.")
                .WithCustomTimestamp()
                .WithTitle(Texts.TICKET_CLOSED)
                .WithUserAsAuthor(ticket.CloserUserInfo)
                .WithColor(Colors.TicketClosedColor)
                .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper(), false)
                .AddField(Texts.OPENED_BY, ticket.OpenerUserInfo.GetMention(), true)
                .AddField(Texts.CLOSED_BY, ticket.CloserUserInfo.GetMention(), true)
                .AddField(Texts.CLOSE_REASON, ticket.CloseReason, true);
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeDeleted(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_DELETED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.TICKET_TYPE, ticketType.Name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeUpdated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_UPDATED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.TICKET_TYPE, ticketType.Name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TicketTypeCreated(TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_CREATED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.TICKET_TYPE, ticketType.Name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamCreated(string teamName, TeamPermissionLevel permissionLevel) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_CREATED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.TEAM_NAME, teamName, true)
                .AddField(Texts.PERMISSION_LEVEL, permissionLevel.ToString(), true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRemoved(string teamName) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_REMOVED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.TEAM_NAME, teamName, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamMemberAdded(DiscordUserInfo userInfo, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_MEMBER_ADDED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, userInfo.GetMention(), true)
                .AddField(Texts.TEAM_NAME, name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamMemberRemoved(DiscordUserInfo userInfo, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_MEMBER_REMOVED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.USER, userInfo.GetMention(), true)
                .AddField(Texts.TEAM_NAME, name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRoleAdded(DiscordRole role, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_ROLE_ADDED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.ROLE, role.Mention, true)
                .AddField(Texts.TEAM_NAME, name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRoleRemoved(DiscordRole role, string name) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_ROLE_REMOVED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.ROLE, role.Mention, true)
                .AddField(Texts.TEAM_NAME, name, true);
    return embed;
  }

  public static DiscordEmbedBuilder TeamRenamed(string oldName, string newName) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TEAM_RENAMED)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.OLD_NAME, oldName, true)
                .AddField(Texts.NEW_NAME, newName, true);
    return embed;
  }

  public static DiscordEmbedBuilder SetupComplete(GuildOption guildOption) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.SETUP_COMPLETE)
                .WithCustomTimestamp()
                .WithColor(Colors.InfoColor)
                .AddField(Texts.GUILD_ID, guildOption.GuildId.ToString(), false)
                .AddField(Texts.GUILD_NAME, guildOption.Name, true)
                .AddField(Texts.CATEGORY_ID, guildOption.CategoryId.ToString(), true)
                .AddField(Texts.LOG_CHANNEL_ID, guildOption.LogChannelId.ToString(), true)
                .AddField(Texts.SENSITIVE_LOGGING, guildOption.IsSensitiveLogging.ToString(), true)
                .AddField(Texts.TAKE_FEEDBACK_AFTER_CLOSING, guildOption.TakeFeedbackAfterClosing.ToString(), true);
    return embed;
  }
}