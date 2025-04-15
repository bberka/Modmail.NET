using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketClosedUserMessageHandler : INotificationHandler<NotifyTicketClosed>
{
	private readonly ModmailBot _bot;
	private readonly IOptions<BotConfig> _options;
	private readonly ISender _sender;

	public NotifyTicketClosedUserMessageHandler(ModmailBot bot,
	                                            IOptions<BotConfig> options,
	                                            ISender sender) {
		_bot = bot;
		_options = options;
		_sender = sender;
	}

	public async Task Handle(NotifyTicketClosed notification, CancellationToken cancellationToken) {
		var option = await _sender.Send(new GetOptionQuery(), cancellationToken);
		Uri? transcriptUri = null;
		if (option.SendTranscriptLinkToUser && option.PublicTranscripts) {
			var sendLinkToUser = Uri.TryCreate(_options.Value.Domain, UriKind.Absolute, out var uri);
			if (sendLinkToUser && uri is not null)
				try {
					transcriptUri = new Uri(uri, "transcript/" + notification.Ticket.Id);
				}
				catch (UriFormatException) {
					transcriptUri = null;
				}
		}

		var pmChannel = await _bot.Client.GetChannelAsync(notification.Ticket.PrivateMessageChannelId);
		var messageBuilder = new DiscordMessageBuilder();
		var embedBuilder = new DiscordEmbedBuilder()
		                   .WithTitle(Lang.YourTicketHasBeenClosed.Translate())
		                   .WithDescription(Lang.YourTicketHasBeenClosedDescription.Translate())
		                   .WithGuildInfoFooter(option)
		                   .WithCustomTimestamp()
		                   .WithColor(ModmailColors.TicketClosedColor);
		var closingMessage = Lang.ClosingMessageDescription.Translate();
		if (!string.IsNullOrEmpty(closingMessage)) embedBuilder.WithDescription(closingMessage);
		if (!string.IsNullOrEmpty(notification.Ticket.CloseReason)) embedBuilder.AddField(Lang.CloseReason.Translate(), notification.Ticket.CloseReason);
		if (transcriptUri is not null) messageBuilder.AddComponents(new DiscordLinkButtonComponent(transcriptUri.AbsoluteUri, Lang.Transcript.Translate()));
		messageBuilder.AddEmbed(embedBuilder);
		await pmChannel.SendMessageAsync(messageBuilder);

		if (option.TakeFeedbackAfterClosing && !notification.DontSendFeedbackMessage) {
			var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
			var starList = new List<DiscordComponent> {
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, notification.Ticket.Id), "1", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, notification.Ticket.Id), "2", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, notification.Ticket.Id), "3", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, notification.Ticket.Id), "4", false, new DiscordComponentEmoji("⭐")),
				new DiscordButtonComponent(DiscordButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, notification.Ticket.Id), "5", false, new DiscordComponentEmoji("⭐"))
			};

			var ticketFeedbackEmbed = new DiscordEmbedBuilder()
			                          .WithTitle(Lang.Feedback.Translate())
			                          .WithDescription(Lang.FeedbackDescription.Translate())
			                          .WithCustomTimestamp()
			                          .WithGuildInfoFooter(option)
			                          .WithColor(ModmailColors.FeedbackColor);

			var response = ticketFeedbackMsgToUser
			               .AddEmbed(ticketFeedbackEmbed)
			               .AddComponents(starList);

			await pmChannel.SendMessageAsync(response);
		}
	}
}