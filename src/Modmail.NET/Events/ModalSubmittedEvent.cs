using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

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

    await args.Interaction.CreateResponseAsync(
                                               DiscordInteractionResponseType.ChannelMessageWithSource,
                                               new DiscordInteractionResponseBuilder().AsEphemeral()
                                                                                      .WithContent(LangProvider.This.GetTranslation(LangKeys.ThankYouForFeedback))
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
          break;
        case "close_ticket_with_reason":
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
    catch (BotExceptionBase ex) {
      Log.Warning(
                  ex,
                  "[{Source}] BotExceptionBase: Error processing modal submission. CustomId: {CustomId}, InteractionId: {InteractionId}",
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
                                                    ticketId,
                                                    args.Interaction.User.Id,
                                                    textInput,
                                                    args.Interaction.Channel
                                                   ));

    Log.Information(
                    "[{Source}] Ticket closed with reason. TicketId: {TicketId}, InteractionId: {InteractionId}",
                    nameof(ModalSubmittedEvent),
                    ticketId,
                    args.Interaction.Id
                   );
  }
}