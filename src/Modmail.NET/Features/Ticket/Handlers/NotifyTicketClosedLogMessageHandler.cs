using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketClosedLogMessageHandler : INotificationHandler<NotifyTicketClosed>
{
	private readonly ISender _sender;

	public NotifyTicketClosedLogMessageHandler(ISender sender) {
		_sender = sender;
	}

	public async ValueTask Handle(NotifyTicketClosed notification, CancellationToken cancellationToken) {
		var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
		var messageBuilder = new DiscordMessageBuilder();
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

		var transcriptUri = UtilTranscript.GetTranscriptUri(notification.Ticket.Id);
		if (transcriptUri is not null) messageBuilder.AddComponents(new DiscordLinkButtonComponent(transcriptUri.AbsoluteUri, Lang.Transcript.Translate()));
		messageBuilder.AddEmbed(embed);
		await logChannel.SendMessageAsync(messageBuilder);
	}
}