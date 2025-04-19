using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketPriorityChangedPrivateMessageHandler : INotificationHandler<NotifyTicketPriorityChanged>
{
	private readonly ISender _sender;
	private readonly ModmailBot _bot;

	public NotifyTicketPriorityChangedPrivateMessageHandler(ISender sender,
	                                                        ModmailBot bot) {
		_sender = sender;
		_bot = bot;
	}

	public async ValueTask Handle(NotifyTicketPriorityChanged notification, CancellationToken cancellationToken) {
		var option = await _sender.Send(new GetOptionQuery(), cancellationToken);
		var privateChannel = await _bot.Client.GetChannelAsync(notification.Ticket.PrivateMessageChannelId);
		var embed = new DiscordEmbedBuilder()
		            .WithServerInfoFooter(option)
		            .WithTitle(Lang.TicketPriorityChanged.Translate())
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TicketPriorityChangedColor)
		            .AddField(Lang.OldPriority.Translate(), notification.OldPriority.ToString(), true)
		            .AddField(Lang.NewPriority.Translate(), notification.NewPriority.ToString(), true);

		var authorUser = await _sender.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		if (!notification.Ticket.Anonymous) embed.WithUserAsAuthor(authorUser);
		await privateChannel.SendMessageAsync(embed);
	}
}