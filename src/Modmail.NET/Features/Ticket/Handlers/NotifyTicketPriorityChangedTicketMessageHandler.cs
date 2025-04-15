using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketPriorityChangedTicketMessageHandler : INotificationHandler<NotifyTicketPriorityChanged>
{
	private readonly ISender _sender;
	private readonly ModmailBot _bot;

	public NotifyTicketPriorityChangedTicketMessageHandler(ISender sender,
	                                                       ModmailBot bot) {
		_sender = sender;
		_bot = bot;
	}

	public async ValueTask Handle(NotifyTicketPriorityChanged notification, CancellationToken cancellationToken) {
		var ticketChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
		var priorityEmoji = notification.NewPriority switch {
			TicketPriority.Normal => TicketConstants.NormalPriorityEmoji,
			TicketPriority.High => TicketConstants.HighPriorityEmoji,
			TicketPriority.Low => TicketConstants.LowPriorityEmoji,
			_ => ""
		};
		var newChName = priorityEmoji + string.Format(TicketConstants.TicketNameTemplate, notification.Ticket.OpenerUser?.Username.Trim());
		await ticketChannel.ModifyAsync(x => { x.Name = newChName; });
		var modUser = await _sender.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.TicketPriorityChanged.Translate())
		            .WithColor(ModmailColors.TicketPriorityChangedColor)
		            .WithCustomTimestamp()
		            .AddField(Lang.OldPriority.Translate(), notification.OldPriority.ToString(), true)
		            .AddField(Lang.NewPriority.Translate(), notification.NewPriority.ToString(), true)
		            .WithUserAsAuthor(modUser);
		await ticketChannel.SendMessageAsync(embed);
	}
}