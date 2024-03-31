using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

public static class ModalSubmitted
{
  [PerformanceLoggerAspect(ThresholdMs = 3000)]
  public static async Task Handle(DiscordClient sender, ModalSubmitEventArgs args) {
    const string logMessage = $"[{nameof(ModalSubmitted)}]{nameof(Handle)}({{CustomId}},{{InteractionId}})";
    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                               new DiscordInteractionResponseBuilder().AsEphemeral().WithContent(LangData.This.GetTranslation(LangKeys.THANK_YOU_FOR_FEEDBACK)));

    try {
      await DiscordUserInfo.AddOrUpdateAsync(args?.Interaction?.User);

      // var interaction = args.Interaction;
      var id = args.Interaction.Data.CustomId;
      var (interactionName, parameters) = UtilInteraction.ParseKey(id);
      switch (interactionName) {
        case "feedback": {
          var textInput = args.Values["feedback"];

          var starParam = parameters[0];
          var ticketIdParam = parameters[1];
          var messageIdParam = parameters[2];

          var starCount = int.Parse(starParam);
          var ticketId = Guid.Parse(ticketIdParam);
          var feedbackMessageId = ulong.Parse(messageIdParam);

          var ticket = await Ticket.GetAsync(ticketId);

          var feedbackMessage = await args.Interaction.Channel.GetMessageAsync(feedbackMessageId);
          await ticket.ProcessAddFeedbackAsync(starCount, textInput, feedbackMessage);
          Log.Information(logMessage, args.Interaction.Data.CustomId, args.Interaction.Id);
          break;
        }
        case "close_ticket_with_reason": {
          var textInput = args.Values["reason"];

          var ticketIdParam = parameters[0];

          var ticketId = Guid.Parse(ticketIdParam);

          var ticket = await Ticket.GetAsync(ticketId);

          await ticket.ProcessCloseTicketAsync(args.Interaction.User.Id, textInput, args.Interaction.Channel);
          Log.Information(logMessage, args.Interaction.Data.CustomId, args.Interaction.Id);
          break;
        }
      }
    }

    catch (BotExceptionBase ex) {
      Log.Warning(ex, logMessage, args.Interaction.Data.CustomId, args.Interaction.Id);
    }
    catch (Exception ex) {
      Log.Error(ex, logMessage, args.Interaction.Data.CustomId, args.Interaction.Id);
    }
  }
}