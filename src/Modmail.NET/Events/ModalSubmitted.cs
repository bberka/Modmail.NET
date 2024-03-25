using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Events;

public static class ModalSubmitted
{
  public static async Task Handle(DiscordClient sender, ModalSubmitEventArgs args) {
    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral().WithContent(Texts.THANK_YOU_FOR_FEEDBACK));

    await DiscordUserInfo.AddOrUpdateAsync(args.Interaction.User);

    var interaction = args.Interaction;
    var id = interaction.Data.CustomId;
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
        if (ticket is null) throw new InvalidOperationException("Ticket not found: " + ticketId);

        var feedbackMessage = await args.Interaction.Channel.GetMessageAsync(feedbackMessageId);
        await ticket.ProcessAddFeedbackAsync(starCount, textInput, feedbackMessage);

        break;
      }
      case "close_ticket_with_reason": {
        var textInput = args.Values["reason"];

        var ticketIdParam = parameters[0];

        var ticketId = Guid.Parse(ticketIdParam);

        var ticket = await Ticket.GetAsync(ticketId);
        if (ticket is null) throw new InvalidOperationException("Ticket not found: " + ticketId);

        await ticket.ProcessCloseTicketAsync(args.Interaction.User.Id, textInput, args.Interaction.Channel);
        break;
      }
    }
  }
}