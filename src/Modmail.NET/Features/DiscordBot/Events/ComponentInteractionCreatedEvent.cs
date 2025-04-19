using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class ComponentInteractionCreatedEvent
{
	[PerformanceLoggerAspect]
	public static async Task ComponentInteractionCreated(
		DiscordClient client,
		ComponentInteractionCreatedEventArgs args
	) {
		Log.Debug(
		          "[{Source}] Component interaction created. CustomId: {CustomId}, UserId: {UserId}, ChannelId: {ChannelId}, InteractionId: {InteractionId}, MessageId: {MessageId}",
		          nameof(ComponentInteractionCreatedEvent),
		          args.Interaction.Data.CustomId,
		          args.User.Id,
		          args.Channel.Id,
		          args.Interaction.Id,
		          args.Message.Id
		         );

		using var scope = client.ServiceProvider.CreateScope();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();

		try {
			await sender.Send(new UpdateDiscordUserCommand(args.User));

			var key = args.Interaction.Data.CustomId;
			var (interactionName, _) = UtilInteraction.ParseKey(key);
			var messageId = args.Message.Id;

			switch (interactionName) {
				case "star":
					await ProcessStarInteraction(args, messageId);
					break;
				case "ticket_type":
					await ProcessTicketTypeInteraction(sender, args, messageId);
					break;
				case "close_ticket": // This must stay due to deprecation and support for existing tickets (v2.0 beta)
				case "close_ticket_with_reason":
					await ProcessCloseTicketInteraction(args);
					break;
				default:
					Log.Warning(
					            "[{Source}] Unknown interaction name: {InteractionName}, CustomId: {CustomId}",
					            nameof(ComponentInteractionCreatedEvent),
					            interactionName,
					            args.Interaction.Data.CustomId
					           );
					break;
			}
		}
		catch (ModmailBotException ex) {
			Log.Warning(
			            ex,
			            "[{Source}] ModmailBotException: Error processing component interaction. CustomId: {CustomId}, UserId: {UserId}, ChannelId: {ChannelId}, InteractionId: {InteractionId}, MessageId: {MessageId}",
			            nameof(ComponentInteractionCreatedEvent),
			            args.Interaction.Data.CustomId,
			            args.User.Id,
			            args.Channel.Id,
			            args.Interaction.Id,
			            args.Message.Id
			           );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Unhandled exception processing component interaction. CustomId: {CustomId}, UserId: {UserId}, ChannelId: {ChannelId}, InteractionId: {InteractionId}, MessageId: {MessageId}",
			          nameof(ComponentInteractionCreatedEvent),
			          args.Interaction.Data.CustomId,
			          args.User.Id,
			          args.Channel.Id,
			          args.Interaction.Id,
			          args.Message.Id
			         );
		}
	}

	private static async Task ProcessStarInteraction(ComponentInteractionCreatedEventArgs args,
	                                                 ulong messageId
	) {
		var key = args.Interaction.Data.CustomId;
		try {
			var (_, parameters) = UtilInteraction.ParseKey(key);
			//feedback process show modal
			var starParam = parameters[0];
			var ticketIdParam = parameters[1];

			var starCount = int.Parse(starParam);
			var ticketId = Guid.Parse(ticketIdParam);

			var feedbackModal = CreateFeedbackModal(starCount, ticketId);

			await args.Interaction.CreateResponseAsync(
			                                           DiscordInteractionResponseType.Modal,
			                                           feedbackModal
			                                          );

			Log.Information(
			                "[{Source}] Star interaction processed. TicketId: {TicketId}, StarCount: {StarCount}, InteractionId: {InteractionId}",
			                nameof(ComponentInteractionCreatedEvent),
			                ticketId,
			                starCount,
			                args.Interaction?.Id
			               );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Error processing star interaction submission. CustomId: {CustomId}, InteractionId: {InteractionId}",
			          nameof(ComponentInteractionCreatedEvent),
			          args.Interaction.Data.CustomId,
			          args.Interaction?.Id
			         );
		}

		return;

		DiscordInteractionResponseBuilder CreateFeedbackModal(int starCount, Guid ticketId) {
			var modal = new DiscordInteractionResponseBuilder()
			            .WithTitle(Lang.Feedback.Translate())
			            .WithCustomId(UtilInteraction.BuildKey("feedback", starCount, ticketId, messageId))
			            .AddComponents(new DiscordTextInputComponent(Lang.Feedback.Translate(),
			                                                         "feedback",
			                                                         Lang.PleaseTellUsReasonsForYourRating.Translate(),
			                                                         style: DiscordTextInputStyle.Paragraph,
			                                                         required: false,
			                                                         max_length: DbLength.Message));
			return modal;
		}
	}

	private static async Task ProcessTicketTypeInteraction(
		ISender sender,
		ComponentInteractionCreatedEventArgs args,
		ulong messageId
	) {
		var key = args.Interaction.Data.CustomId;
		try {
			await args.Interaction.CreateResponseAsync(
			                                           DiscordInteractionResponseType.UpdateMessage
			                                          );
			var (_, parameters) = UtilInteraction.ParseKey(key);

			var ticketIdParam = parameters[0];
			var ticketId = Guid.Parse(ticketIdParam);
			var selectedTypeKey = args.Values.FirstOrDefault();
			if (string.IsNullOrEmpty(selectedTypeKey)) {
				Log.Warning(
				            "[{Source}] No ticket type selected. InteractionId: {InteractionId}, MessageId: {MessageId}",
				            nameof(ComponentInteractionCreatedEvent),
				            args.Interaction?.Id,
				            messageId
				           );
				return;
			}

			await sender.Send(new ProcessChangeTicketTypeCommand(args.User.Id,
			                                                     ticketId,
			                                                     selectedTypeKey
			                                                    ));

			Log.Information(
			                "[{Source}] Ticket type changed. TicketId: {TicketId}, SelectedTypeKey: {SelectedTypeKey}, InteractionId: {InteractionId}",
			                nameof(ComponentInteractionCreatedEvent),
			                ticketId,
			                selectedTypeKey,
			                args.Interaction?.Id
			               );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Error processing process ticket type interaction. CustomId: {CustomId}, InteractionId: {InteractionId}",
			          nameof(ComponentInteractionCreatedEvent),
			          args.Interaction.Data.CustomId,
			          args.Interaction?.Id
			         );
		}
	}

	private static async Task ProcessCloseTicketInteraction(ComponentInteractionCreatedEventArgs args
	) {
		var key = args.Interaction.Data.CustomId;
		try {
			var (_, parameters) = UtilInteraction.ParseKey(key);

			var ticketIdParam = parameters[0];
			var ticketId = Guid.Parse(ticketIdParam);
			var modal = CreateCloseTicketWithReasonModal(ticketId);
			await args.Interaction.CreateResponseAsync(
			                                           DiscordInteractionResponseType.Modal,
			                                           modal
			                                          );

			Log.Information(
			                "[{Source}] Close ticket interaction triggered. TicketId: {TicketId}, InteractionId: {InteractionId}",
			                nameof(ComponentInteractionCreatedEvent),
			                ticketId,
			                args.Interaction?.Id
			               );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Error processing  process close ticket interaction. CustomId: {CustomId}, InteractionId: {InteractionId}",
			          nameof(ComponentInteractionCreatedEvent),
			          args.Interaction.Data.CustomId,
			          args.Interaction?.Id
			         );
		}

		return;

		DiscordInteractionResponseBuilder CreateCloseTicketWithReasonModal(Guid ticketId) {
			var modal = new DiscordInteractionResponseBuilder()
			            .WithTitle(Lang.CloseTicket.Translate())
			            .WithCustomId(UtilInteraction.BuildKey("close_ticket_with_reason", ticketId))
			            .AddComponents(new DiscordTextInputComponent(Lang.Reason.Translate(),
			                                                         "reason",
			                                                         Lang.EnterReasonForClosingThisTicket.Translate(),
			                                                         style: DiscordTextInputStyle.Paragraph,
			                                                         required: false,
			                                                         max_length: DbLength.Reason));
			return modal;
		}
	}
}