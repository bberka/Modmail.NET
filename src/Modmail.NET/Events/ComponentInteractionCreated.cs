using DSharpPlus;
using DSharpPlus.EventArgs;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Utils;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.User);
    var interaction = args.Interaction;
    var key = interaction.Data.CustomId;
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
        break;
      }
      case "ticket_type": {
        //for user ticket change type allowed only once for user
        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);
        var selectedTypeKey = args.Values.FirstOrDefault();
        if (string.IsNullOrEmpty(selectedTypeKey)) {
          return;
        }

        var ticket = await Ticket.GetActiveAsync(ticketId);
        if (ticket is not null) {
          await ticket.ProcessChangeTicketTypeAsync(args.User.Id, selectedTypeKey, null, args.Channel, args.Message);
        }

        break;
      }
      case "close_ticket": {
        //close ticket with button
        await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);
        var ticket = await Ticket.GetActiveAsync(ticketId);
        if (ticket is not null) {
          await ticket.ProcessCloseTicketAsync(args.User.Id, null, args.Channel);
        }

        break;
      }
      case "close_ticket_with_reason": {
        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);

        break;
      }
    }

    await Task.CompletedTask;
  }
}