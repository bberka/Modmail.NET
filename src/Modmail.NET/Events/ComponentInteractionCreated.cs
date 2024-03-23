using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Modmail.NET.Abstract.Services;
using Modmail.NET.Common;
using Modmail.NET.Static;
using Serilog;

namespace Modmail.NET.Events;

public static class ComponentInteractionCreated
{
  public static async Task Handle(DiscordClient sender, ComponentInteractionCreateEventArgs args) {
    var interaction = args.Interaction;
    var id = interaction.Data.CustomId;
    var (interactionName, parameters) = UtilInteraction.ParseKey(id);
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

        var dbService = ServiceLocator.Get<IDbService>();
        var ticketType = await dbService.GetTicketTypeByKeyAsync(selectedTypeKey);
        if (ticketType == null) {
          Log.Warning("Ticket type {TicketTypeKey} was not found for ticket {TicketId}", selectedTypeKey, ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          return;
        }

        var ticket = await dbService.GetActiveTicketAsync(ticketId);
        if (ticket == null) {
          Log.Warning("Ticket {TicketId} was not found", ticketId);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage);
          return;
        }

        var guild = await ModmailBot.This.Client.GetGuildAsync(MMConfig.This.MainServerId);

        var guildOption = await dbService.GetOptionAsync(guild.Id);
        if (guildOption == null) {
          var embed1 = ModmailEmbeds.Base(Texts.SERVER_NOT_SETUP, "", DiscordColor.Red);
          await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                                     new DiscordInteractionResponseBuilder()
                                                       .AddEmbed(embed1)
                                                       .AsEphemeral());
          return;
        }

        ticket.TicketTypeId = ticketType.Id;

        await dbService.UpdateTicketAsync(ticket);

        await args.Message.ModifyAsync(x => { x.Embed = ModmailEmbeds.ToUser.TicketCreatedUpdated(guild, guildOption, ticketId, ticketType); });

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
          var embed2 = ModmailEmbeds.ToMail.TicketTypeSelected(args.User, ticketType);
          await mailChannel.SendMessageAsync(embed2);
        }
        else {
          Log.Error("Mail channel {MailChannelId} not found", ticket.ModMessageChannelId);
        }

        break;
      }
    }

    await Task.CompletedTask;
  }
}