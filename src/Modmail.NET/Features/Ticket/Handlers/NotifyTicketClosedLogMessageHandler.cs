using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketClosedLogMessageHandler : INotificationHandler<NotifyTicketClosed>
{
	private readonly ModmailBot _bot;
	private readonly ISender _sender;

	public NotifyTicketClosedLogMessageHandler(ModmailBot bot,
	                                           ISender sender) {
		_bot = bot;
		_sender = sender;
	}

	public async Task Handle(NotifyTicketClosed notification, CancellationToken cancellationToken) {
		var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithCustomTimestamp()
		            .WithTitle(Lang.TicketClosed.Translate())
		            .WithColor(ModmailColors.TicketClosedColor)
		            .AddField(Lang.TicketId.Translate(), notification.Ticket.Id.ToString().ToUpper())
		            .AddField(Lang.OpenedBy.Translate(), notification.Ticket.OpenerUser!.GetMention())
		            .AddField(Lang.ClosedBy.Translate(), notification.Ticket.CloserUser!.GetMention())
		            .AddField(Lang.TicketPriority.Translate(), notification.Ticket.Priority.ToString());
		if (notification.Ticket.OpenerUser is not null) embed.WithUserAsAuthor(notification.Ticket.CloserUser);
		if (!string.IsNullOrEmpty(notification.Ticket.CloseReason)) embed.AddField(Lang.CloseReason.Translate(), notification.Ticket.CloseReason);
		await logChannel.SendMessageAsync(embed);
	}
}