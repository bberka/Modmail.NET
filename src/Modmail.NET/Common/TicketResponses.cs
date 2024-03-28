using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Models;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

/// <summary>
/// Contains the embed messages bot to send to ticket channels
/// </summary>
public static class TicketResponses
{
  public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId, List<PermissionInfo> permissionInfos) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.NEW_TICKET)
                .WithCustomTimestamp()
                .WithDescription(Texts.NEW_TICKET_DESCRIPTION_MESSAGE)
                .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
                .AddField(Texts.USER, member.Mention, true)
                .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true)
                .WithColor(Colors.TicketCreatedColor);

    var messageBuilder = new DiscordMessageBuilder()
                         .AddEmbed(embed)
                         .AddComponents(new DiscordButtonComponent(ButtonStyle.Danger,
                                                                   UtilInteraction.BuildKey("close_ticket", ticketId.ToString()),
                                                                   Texts.CLOSE_TICKET,
                                                                   emoji: new DiscordComponentEmoji("🔒")),
                                        new DiscordButtonComponent(ButtonStyle.Danger,
                                                                   UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
                                                                   Texts.CLOSE_TICKET_WITH_REASON,
                                                                   emoji: new DiscordComponentEmoji("🔒"))
                                       );

    var sb = new StringBuilder();
    foreach (var permissionInfo in permissionInfos.Where(permissionInfo => permissionInfo.PingOnNewTicket)) {
      sb.AppendLine(permissionInfo.GetMention());
    }

    messageBuilder.WithContent(sb.ToString());
    return messageBuilder;
  }

  public static DiscordEmbedBuilder NoteAdded(TicketNote note, DiscordUserInfo user) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.NOTE_ADDED)
                .WithDescription(note.Content)
                .WithColor(Colors.NoteAddedColor)
                .WithCustomTimestamp()
                .WithUserAsAuthor(user);
    return embed;
  }

  public static DiscordEmbedBuilder AnonymousToggled(Ticket ticket) {
    var embed2 = new DiscordEmbedBuilder()
                 .WithTitle(ticket.Anonymous
                              ? Texts.ANONYMOUS_MOD_ON
                              : Texts.ANONYMOUS_MOD_OFF)
                 .WithColor(Colors.AnonymousToggledColor)
                 .WithCustomTimestamp()
                 .WithUserAsAuthor(ticket.OpenerUserInfo)
                 .WithDescription(ticket.Anonymous
                                    ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                                    : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);
    return embed2;
  }

  public static DiscordEmbedBuilder TicketTypeChanged(DiscordUserInfo user, TicketType ticketType) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_TYPE_CHANGED)
                .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                .WithUserAsAuthor(user)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketTypeChangedColor);
    return embed;
  }

  public static DiscordEmbedBuilder TicketPriorityChanged(DiscordUserInfo modUser, Ticket ticket, TicketPriority oldPriority, TicketPriority newPriority) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                .WithColor(Colors.TicketPriorityChangedColor)
                .WithCustomTimestamp()
                .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true)
                .WithUserAsAuthor(modUser);
    return embed;
  }

  public static DiscordEmbedBuilder MessageSent(DiscordMessage message, bool anonymous) {
    var embed = new DiscordEmbedBuilder()
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(Colors.MessageSentColor)
                .WithUserAsAuthor(message.Author)
                .AddAttachment(message.Attachments);

    if (anonymous) {
      embed.WithFooter(Texts.THIS_MESSAGE_SENT_ANONYMOUSLY);
    }

    return embed;
  }


  public static DiscordMessageBuilder MessageReceived(DiscordMessage message, List<PermissionInfo>? permissions = null) {
    var embed = new DiscordEmbedBuilder()
                .WithDescription(message.Content)
                .WithCustomTimestamp()
                .WithColor(Colors.MessageReceivedColor)
                .AddAttachment(message.Attachments)
                .WithUserAsAuthor(message.Author);

    var msgBuilder = new DiscordMessageBuilder()
      .AddEmbed(embed);
    if (permissions is not null) {
      var sb = new StringBuilder();
      foreach (var permissionInfo in permissions.Where(x => x.PingOnNewMessage)) {
        sb.AppendLine(permissionInfo.GetMention());
      }

      msgBuilder.WithContent(sb.ToString());
    }

    return msgBuilder;
  }
}