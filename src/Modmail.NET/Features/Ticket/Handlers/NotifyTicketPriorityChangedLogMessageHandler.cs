using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketPriorityChangedLogMessageHandler : INotificationHandler<NotifyTicketPriorityChanged>
{
	private readonly ISender _sender;

	public NotifyTicketPriorityChangedLogMessageHandler(ISender sender) {
		_sender = sender;
	}

	public async ValueTask Handle(NotifyTicketPriorityChanged notification, CancellationToken cancellationToken) {
		var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.TicketPriorityChanged.Translate())
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TicketPriorityChangedColor)
		            .AddField(Lang.TicketId.Translate(), notification.Ticket.Id.ToString().ToUpper())
		            .AddField(Lang.OldPriority.Translate(), notification.OldPriority.ToString(), true)
		            .AddField(Lang.NewPriority.Translate(), notification.NewPriority.ToString(), true);
		var modUser = await _sender.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		if (!notification.Ticket.Anonymous) embed.WithUserAsAuthor(modUser);
		await logChannel.SendMessageAsync(embed);
	}
}