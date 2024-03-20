using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Modmail.NET.Common;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    
    var interaction = args.Interaction;
    var id = interaction.Data.CustomId;
    var (interactionName, parameters) = UtilInteraction.ParseKey(id);

    switch (interactionName) {
      case "star":
        var starParam = parameters[0];
        var ticketIdParam = parameters[1];

        var starCount = int.Parse(starParam);
        var ticketId = Guid.Parse(ticketIdParam);
        var messageId = args.Message.Id;
        
        var feedbackModal = ModmailInteractions.CreateFeedbackModal(starCount, ticketId, messageId);

        await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, feedbackModal);

        break;
    }

    await Task.CompletedTask;
  }
}