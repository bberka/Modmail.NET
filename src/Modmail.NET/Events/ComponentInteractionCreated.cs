using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Aspects;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  [PerformanceLoggerAspect]
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    const string logMessage = $"[{nameof(ComponentInteractionCreated)}]{nameof(Handle)}({{CustomId}},{{UserId}},{{ChannelId}},{{InteractionId}},{{MessageId}})";
    try {
      await DiscordUserInfo.AddOrUpdateAsync(args.User);
      var interaction = args.Interaction;
      var key = interaction?.Data?.CustomId;
      var (interactionName, parameters) = UtilInteraction.ParseKey(key);
      var messageId = args.Message.Id;

      switch (interactionName) {
        case "star": {
          //feedback process show modal
          var starParam = parameters[0];
          var ticketIdParam = parameters[1];

          var starCount = int.Parse(starParam);
          var ticketId = Guid.Parse(ticketIdParam);

          var feedbackModal = Modals.CreateFeedbackModal(starCount, ticketId, messageId);

          await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, feedbackModal);
          Log.Information(logMessage,
                          args.Interaction?.Data?.CustomId,
                          args.User?.Id,
                          args.Channel?.Id,
                          args.Interaction?.Id,
                          args.Message?.Id);
          break;
        }
        case "ticket_type": {
          //for user ticket change type allowed only once for user
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          var selectedTypeKey = args.Values.FirstOrDefault();
          if (string.IsNullOrEmpty(selectedTypeKey)) break;

          var ticket = await Ticket.GetActiveTicketAsync(ticketId);
          await ticket.ProcessChangeTicketTypeAsync(args.User.Id, selectedTypeKey, null, args.Channel, args.Message);
          Log.Information(logMessage,
                          args.Interaction?.Data?.CustomId,
                          args.User?.Id,
                          args.Channel?.Id,
                          args.Interaction?.Id,
                          args.Message?.Id);
          break;
        }
        case "close_ticket": {
          //close ticket with button
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          var ticket = await Ticket.GetActiveTicketAsync(ticketId);
          await ticket.ProcessCloseTicketAsync(args.User.Id, null, args.Channel);
          Log.Information(logMessage,
                          args.Interaction?.Data?.CustomId,
                          args.User?.Id,
                          args.Channel?.Id,
                          args.Interaction?.Id,
                          args.Message?.Id);
          break;
        }
        case "close_ticket_with_reason": {
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          var modal = Modals.CreateCloseTicketWithReasonModal(ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

          Log.Information(logMessage,
                          args.Interaction?.Data?.CustomId,
                          args.User?.Id,
                          args.Channel?.Id,
                          args.Interaction?.Id,
                          args.Message?.Id);
          break;
        }
      }
    }


    catch (BotExceptionBase ex) {
      Log.Warning(ex,
                  logMessage,
                  args.Interaction?.Data?.CustomId,
                  args.User?.Id,
                  args.Channel?.Id,
                  args.Interaction?.Id,
                  args.Message?.Id);
    }
    catch (Exception ex) {
      Log.Fatal(ex,
                logMessage,
                args.Interaction?.Data?.CustomId,
                args.User?.Id,
                args.Channel?.Id,
                args.Interaction?.Id,
                args.Message?.Id);
    }
  }
}