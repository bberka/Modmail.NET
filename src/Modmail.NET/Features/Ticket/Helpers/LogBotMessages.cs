using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Helpers;

/// <summary>
///   Contains the embed messages bot to send to log channel
/// </summary>
public static class LogBotMessages
{
	public static DiscordEmbedBuilder NewTicketCreated(DiscordMessage initialMessage,
	                                                   Guid ticketId) {
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.NewTicketCreated.Translate())
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TicketCreatedColor)
		            .AddField(Lang.TicketId.Translate(), ticketId.ToString().ToUpper())
		            .AddField(Lang.User.Translate(), initialMessage.Author!.Mention);
		if (initialMessage.Author is not null) embed.WithUserAsAuthor(initialMessage.Author);

		return embed;
	}

	public static DiscordEmbedBuilder TicketClosed(Database.Entities.Ticket ticket) {
		var embed = new DiscordEmbedBuilder()
		            .WithCustomTimestamp()
		            .WithTitle(Lang.TicketClosed.Translate())
		            .WithColor(ModmailColors.TicketClosedColor)
		            .AddField(Lang.TicketId.Translate(), ticket.Id.ToString().ToUpper())
		            .AddField(Lang.OpenedBy.Translate(), ticket.OpenerUser!.GetMention(), true)
		            .AddField(Lang.ClosedBy.Translate(), ticket.CloserUser!.GetMention(), true)
		            .AddField(Lang.TicketPriority.Translate(), ticket.Priority.ToString(), true);
		if (ticket.OpenerUser is not null) embed.WithUserAsAuthor(ticket.CloserUser);
		if (!string.IsNullOrEmpty(ticket.CloseReason)) embed.AddField(Lang.CloseReason.Translate(), ticket.CloseReason, true);

		return embed;
	}
}