using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class ModalSubmittedEvent
{
	[PerformanceLoggerAspect]
	public static async Task ModalSubmitted(
		DiscordClient client,
		ModalSubmittedEventArgs args
	) {
		Log.Debug(
		          "[{Source}] Modal submitted. CustomId: {CustomId}, InteractionId: {InteractionId}",
		          nameof(ModalSubmittedEvent),
		          args.Interaction.Data.CustomId,
		          args.Interaction.Id
		         );


		using var scope = client.ServiceProvider.CreateScope();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();

		try {
			await sender.Send(new UpdateDiscordUserCommand(args.Interaction.User));

			var (interactionName, parameters) =
				UtilInteraction.ParseKey(args.Interaction.Data.CustomId);

			switch (interactionName) {
				case "feedback":
					await ProcessFeedback(sender, args, parameters);
					await args.Interaction.CreateResponseAsync(
					                                           DiscordInteractionResponseType.ChannelMessageWithSource,
					                                           new DiscordInteractionResponseBuilder().AsEphemeral()
					                                                                                  .WithContent(LangProvider.This.GetTranslation(Lang.ThankYouForFeedback))
					                                          );
					break;
				case "close_ticket_with_reason":
					await args.Interaction.CreateResponseAsync(
					                                           DiscordInteractionResponseType.DeferredMessageUpdate
					                                          );

					await ProcessCloseTicketWithReason(sender, args, parameters);
					break;
				default:
					Log.Warning(
					            "[{Source}] Unknown interaction name: {InteractionName}, CustomId: {CustomId}",
					            nameof(ModalSubmittedEvent),
					            interactionName,
					            args.Interaction.Data.CustomId
					           );
					break;
			}
		}
		catch (ModmailBotException ex) {
			Log.Warning(
			            ex,
			            "[{Source}] ModmailBotException: Error processing modal submission. CustomId: {CustomId}, InteractionId: {InteractionId}",
			            nameof(ModalSubmittedEvent),
			            args.Interaction.Data.CustomId,
			            args.Interaction.Id
			           );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Unhandled exception processing modal submission. CustomId: {CustomId}, InteractionId: {InteractionId}",
			          nameof(ModalSubmittedEvent),
			          args.Interaction.Data.CustomId,
			          args.Interaction.Id
			         );
		}
	}

	private static async Task ProcessFeedback(
		ISender sender,
		ModalSubmittedEventArgs args,
		string[] parameters
	) {
		var textInput = args.Values["feedback"];

		var starParam = parameters[0];
		var ticketIdParam = parameters[1];
		var messageIdParam = parameters[2];

		var starCount = int.Parse(starParam);
		var ticketId = Guid.Parse(ticketIdParam);
		var feedbackMessageId = ulong.Parse(messageIdParam);

		var feedbackMessage = await args.Interaction.Channel.GetMessageAsync(feedbackMessageId);
		await sender.Send(new ProcessAddFeedbackCommand(
		                                                ticketId,
		                                                starCount,
		                                                textInput,
		                                                feedbackMessage
		                                               ));

		Log.Information(
		                "[{Source}] Feedback processed. TicketId: {TicketId}, StarCount: {StarCount}, InteractionId: {InteractionId}",
		                nameof(ModalSubmittedEvent),
		                ticketId,
		                starCount,
		                args.Interaction.Id
		               );
	}

	private static async Task ProcessCloseTicketWithReason(
		ISender sender,
		ModalSubmittedEventArgs args,
		string[] parameters
	) {
		var textInput = args.Values["reason"];
		var ticketIdParam = parameters[0];
		var ticketId = Guid.Parse(ticketIdParam);

		await sender.Send(new ProcessCloseTicketCommand(
		                                                args.Interaction.User.Id,
		                                                ticketId,
		                                                textInput
		                                               ));

		Log.Information(
		                "[{Source}] Ticket closed with reason. TicketId: {TicketId}, InteractionId: {InteractionId}",
		                nameof(ModalSubmittedEvent),
		                ticketId,
		                args.Interaction.Id
		               );
	}
}