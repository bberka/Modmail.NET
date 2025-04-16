using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketCreatedLogMessageHandler : INotificationHandler<NotifyTicketCreated>
{
	private readonly ISender _sender;

	public NotifyTicketCreatedLogMessageHandler(ISender sender) {
		_sender = sender;
	}

	public async ValueTask Handle(NotifyTicketCreated notification, CancellationToken cancellationToken) {
		var embed = new DiscordEmbedBuilder()
		            .WithTitle(Lang.NewTicketCreated.Translate())
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TicketCreatedColor)
		            .AddField(Lang.TicketId.Translate(), notification.TicketId.ToString().ToUpper())
		            .AddField(Lang.User.Translate(), notification.User.Mention);
		if (notification.Message.Author is not null) embed.WithUserAsAuthor(notification.Message.Author);
		var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		await logChannel.SendMessageAsync(embed);
	}
}