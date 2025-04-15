using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketTypeChangedTicketMessageHandler : INotificationHandler<NotifyTicketTypeChanged>
{
	private readonly ModmailBot _bot;
	private readonly ISender _sender;

	public NotifyTicketTypeChangedTicketMessageHandler(ModmailBot bot,
	                                                   ISender sender) {
		_bot = bot;
		_sender = sender;
	}

	public async Task Handle(NotifyTicketTypeChanged notification, CancellationToken cancellationToken) {
		var ticketChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
		var author = await _sender.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.TicketTypeChanged.Translate())
		            .WithUserAsAuthor(author)
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TicketTypeChangedColor)
		            .WithDescription(notification.TicketType is not null
			                             ? string.Format(Lang.TicketTypeSet.Translate(), notification.TicketType.Emoji, notification.TicketType.Name)
			                             : Lang.TicketTypeRemoved.Translate());
		await ticketChannel.SendMessageAsync(embed);
	}
}