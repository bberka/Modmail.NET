using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    var interaction = args.Interaction;
    var key = interaction.Data.CustomId;
    var (interactionName, parameters) = UtilInteraction.ParseKey(key);
    var messageId = args.Message.Id;

    switch (interactionName) {
      case "star": {
        var starParam = parameters[0];
        var ticketIdParam = parameters[1];

        var starCount = int.Parse(starParam);
        var ticketId = Guid.Parse(ticketIdParam);


        var feedbackModal = ModmailInteractions.CreateFeedbackModal(starCount, ticketId, messageId);

        await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, feedbackModal);
        break;
      }
      case "ticket_type": {
        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);
        var selectedTypeKey = args.Values.FirstOrDefault();
        if (string.IsNullOrEmpty(selectedTypeKey)) {
          Log.Warning("No ticket type key was selected for ticket {TicketId}", ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          return;
        }

        var ticketType = await TicketType.GetByKeyAsync(selectedTypeKey);
        if (ticketType == null) {
          Log.Warning("Ticket type {TicketTypeKey} was not found for ticket {TicketId}", selectedTypeKey, ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          return;
        }

        var ticket = await Ticket.GetActiveAsync(ticketId);
        if (ticket == null) {
          Log.Warning("Ticket {TicketId} was not found", ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          return;
        }

        var guild = await ModmailBot.This.Client.GetGuildAsync(MMConfig.This.MainServerId);

        var guildOption = await GuildOption.GetAsync();
        if (guildOption == null) {
          var embed1 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, "", DiscordColor.Red);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                     new DiscordInteractionResponseBuilder()
                                                       .AddEmbed(embed1)
                                                       .AsEphemeral());
          return;
        }

        ticket.TicketTypeId = ticketType.Id;

        await ticket.UpdateAsync();

        await args.Message.ModifyAsync(x => { x.Embed = ModmailEmbeds.ToUser.TicketCreatedUpdated(guild, guildOption, ticketType); });

        TicketTypeSelectionTimeoutMgr.This.RemoveMessage(args.Message.Id);

        var logChannel = await ModmailBot.This.Client.GetChannelAsync(guildOption.LogChannelId);
        if (logChannel is not null) {
          var embed = ModmailEmbeds.ToLog.TicketTypeSelected(args.User, ticketType, ticket);
          await logChannel.SendMessageAsync(embed);
        }
        else {
          Log.Error("Log channel {LogChannelId} not found", guildOption.LogChannelId);
        }

        var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ticket.ModMessageChannelId);
        if (mailChannel is not null) {
          var embed2 = ModmailEmbeds.ToMail.TicketTypeChanged(args.User, ticketType);
          await mailChannel.SendMessageAsync(embed2);
        }
        else {
          Log.Error("Mail channel {MailChannelId} not found", ticket.ModMessageChannelId);
        }

        var embed3 = ModmailEmbeds.ToUser.TicketTypeEmbedMessage(ticketType);
        if (embed3 is not null) {
          await args.Channel.SendMessageAsync(embed3);
        }

        break;
      }
      case "close_ticket": {
        await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);

        // 
        var ticket = await Ticket.GetActiveAsync(ticketId);
        if (ticket is not null) {
          await args.Interaction.EditOriginalResponseAsync(ModmailEmbeds.Webhook.Success(Texts.TICKET_CLOSED));
          await ticket.CloseTicketAsync(args.User.Id, null, args.Channel);
        }
        else {
          //TODO: Handle ticket not found
        }

        break;
      }
      case "close_ticket_with_reason": {
        var ticketIdParam = parameters[0];
        var ticketId = Guid.Parse(ticketIdParam);

        //create modal

        //TODO:

        break;
      }
    }

    await Task.CompletedTask;
  }
}