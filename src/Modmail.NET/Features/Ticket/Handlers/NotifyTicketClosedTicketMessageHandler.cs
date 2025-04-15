using MediatR;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketClosedTicketMessageHandler : INotificationHandler<NotifyTicketClosed>
{
	private readonly ModmailBot _bot;

	public NotifyTicketClosedTicketMessageHandler(ModmailBot bot) {
		_bot = bot;
	}

	public async Task Handle(NotifyTicketClosed notification, CancellationToken cancellationToken) {
		var modChatChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
		await modChatChannel.DeleteAsync(Lang.TicketClosed.Translate());
	}
}