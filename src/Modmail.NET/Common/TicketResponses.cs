using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Models;

namespace Modmail.NET.Common;

/// <summary>
///     Contains the embed messages bot to send to ticket channels
/// </summary>
public static class TicketResponses
{
    public static DiscordMessageBuilder NewTicket(
        DiscordUser member,
        Guid ticketId,
        List<PermissionInfo> permissionInfos
    )
    {
        var embed = new DiscordEmbedBuilder().WithTitle(LangKeys.NEW_TICKET.GetTranslation())
            .WithCustomTimestamp()
            .WithDescription(LangKeys.NEW_TICKET_DESCRIPTION_MESSAGE.GetTranslation())
            .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
            .AddField(LangKeys.USER.GetTranslation(), member.Mention, true)
            .AddField(LangKeys.TICKET_ID.GetTranslation(), ticketId.ToString()
                .ToUpper(), true)
            .WithColor(Colors.TicketCreatedColor);

        var messageBuilder = new DiscordMessageBuilder().AddEmbed(embed)
            .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Danger, UtilInteraction.BuildKey("close_ticket", ticketId.ToString()),
                    LangKeys.CLOSE_TICKET.GetTranslation(), emoji: new DiscordComponentEmoji("🔒")),
                new DiscordButtonComponent(ButtonStyle.Danger, UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
                    LangKeys.CLOSE_TICKET_WITH_REASON.GetTranslation(), emoji: new DiscordComponentEmoji("🔒")));

        var sb = new StringBuilder();
        foreach (var permissionInfo in permissionInfos.Where(permissionInfo => permissionInfo.PingOnNewTicket))
            sb.AppendLine(permissionInfo.GetMention());

        messageBuilder.WithContent(sb.ToString());
        return messageBuilder;
    }

    public static DiscordEmbedBuilder NoteAdded(TicketNote note, DiscordUserInfo user)
    {
        var embed = new DiscordEmbedBuilder().WithTitle(LangKeys.NOTE_ADDED.GetTranslation())
            .WithDescription(note.Content)
            .WithColor(Colors.NoteAddedColor)
            .WithCustomTimestamp()
            .WithUserAsAuthor(user);
        return embed;
    }

    public static DiscordEmbedBuilder AnonymousToggled(Ticket ticket)
    {
        var embed2 = new DiscordEmbedBuilder()
            .WithTitle(ticket.Anonymous ? LangKeys.ANONYMOUS_MOD_ON.GetTranslation() : LangKeys.ANONYMOUS_MOD_OFF.GetTranslation())
            .WithColor(Colors.AnonymousToggledColor)
            .WithCustomTimestamp()
            .WithDescription(ticket.Anonymous
                ? LangKeys.TICKET_SET_ANONYMOUS_DESCRIPTION.GetTranslation()
                : LangKeys.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION.GetTranslation());

        if (ticket.OpenerUser is not null) embed2.WithUserAsAuthor(ticket.OpenerUser);


        return embed2;
    }

    public static DiscordEmbedBuilder TicketTypeChanged(DiscordUserInfo user, TicketType? ticketType)
    {
        var embed = new DiscordEmbedBuilder().WithTitle(LangKeys.TICKET_TYPE_CHANGED.GetTranslation())
            .WithUserAsAuthor(user)
            .WithCustomTimestamp()
            .WithColor(Colors.TicketTypeChangedColor);
        if (ticketType is not null)
            embed.WithDescription(string.Format(LangKeys.TICKET_TYPE_SET.GetTranslation(), ticketType.Emoji, ticketType.Name));
        else
            embed.WithDescription(LangKeys.TICKET_TYPE_REMOVED.GetTranslation());

        return embed;
    }

    public static DiscordEmbedBuilder TicketPriorityChanged(
        DiscordUserInfo modUser,
        Ticket ticket,
        TicketPriority oldPriority,
        TicketPriority newPriority
    )
    {
        var embed = new DiscordEmbedBuilder().WithTitle(LangKeys.TICKET_PRIORITY_CHANGED.GetTranslation())
            .WithColor(Colors.TicketPriorityChangedColor)
            .WithCustomTimestamp()
            .AddField(LangKeys.OLD_PRIORITY.GetTranslation(), oldPriority.ToString(), true)
            .AddField(LangKeys.NEW_PRIORITY.GetTranslation(), newPriority.ToString(), true)
            .WithUserAsAuthor(modUser);
        return embed;
    }

    public static DiscordEmbedBuilder MessageSent(DiscordMessage message, bool anonymous)
    {
        var embed = new DiscordEmbedBuilder().WithDescription(message.Content)
            .WithCustomTimestamp()
            .WithColor(Colors.MessageSentColor)
            .WithUserAsAuthor(message.Author)
            .AddAttachment(message.Attachments);

        if (anonymous) embed.WithFooter(LangKeys.THIS_MESSAGE_SENT_ANONYMOUSLY.GetTranslation());

        return embed;
    }


    public static DiscordMessageBuilder MessageReceived(DiscordMessage message, List<PermissionInfo>? permissions = null)
    {
        var embed = new DiscordEmbedBuilder().WithDescription(message.Content)
            .WithCustomTimestamp()
            .WithColor(Colors.MessageReceivedColor)
            .AddAttachment(message.Attachments)
            .WithUserAsAuthor(message.Author);

        var msgBuilder = new DiscordMessageBuilder().AddEmbed(embed);
        if (permissions is not null)
        {
            var sb = new StringBuilder();
            foreach (var permissionInfo in permissions.Where(x => x.PingOnNewMessage)) sb.AppendLine(permissionInfo.GetMention());

            msgBuilder.WithContent(sb.ToString());
        }

        return msgBuilder;
    }
}