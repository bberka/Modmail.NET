using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Entities.AuditLogs;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Abstract;
using Modmail.NET.Aspects;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Models.Dto;
using Modmail.NET.Queues;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET;

public class ModmailEventHandlers
{
  public static async Task InteractionCreated(DiscordClient client, InteractionCreatedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Interaction?.User));
  }

  public static async Task OnUserUpdated(DiscordClient client, UserUpdatedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.UserAfter));
  }

  public static async Task OnUserSettingsUpdated(DiscordClient client, UserSettingsUpdatedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.User));
  }

  public static async Task OnMessageDeleted(DiscordClient client, MessageDeletedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Message.Author));
  }

  public static async Task OnMessageReactionAdded(DiscordClient client, MessageReactionAddedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.User));
  }

  public static async Task OnMessageUpdated(DiscordClient client, MessageUpdatedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Author));
  }

  public static async Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Member));
  }

  public static async Task OnGuildMemberRemoved(DiscordClient client, GuildMemberRemovedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Member));
  }

  public static async Task OnMessageCreated(DiscordClient client, MessageCreatedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Message.Author));
    if (args.Message.Author?.IsBot == true) return;
    if (args.Message.IsTTS) return;

    var ticketMessageQueue = scope.ServiceProvider.GetRequiredService<TicketMessageQueue>();
    await ticketMessageQueue.Enqueue(args.Author.Id, new DiscordTicketMessageDto(client, args));
  }

  public static async Task OnGuildBanAdded(DiscordClient client, GuildBanAddedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Member));
  }

  public static async Task OnGuildBanRemoved(DiscordClient client, GuildBanRemovedEventArgs args) {
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Member));
  }

  [PerformanceLoggerAspect]
  public static async Task OnChannelDeleted(DiscordClient client, ChannelDeletedEventArgs args) {
    const string logMessage = $"[{nameof(OnChannelDeleted)}]({{ChannelId}})";
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var langData = scope.ServiceProvider.GetRequiredService<LangProvider>();
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
    if (ticketId != Guid.Empty)
      try {
        var auditLogEntry = await args.Guild.GetAuditLogsAsync(1, null, DiscordAuditLogActionType.ChannelDelete).FirstOrDefaultAsync();
        var user = auditLogEntry?.UserResponsible ?? client.CurrentUser;
        await sender.Send(new UpdateDiscordUserCommand(user));
        var ticket = await sender.Send(new GetTicketQuery(ticketId, true));
        if (ticket is not null) {
          if (ticket.ClosedDateUtc.HasValue) return; // Ticket is already closed
          await sender.Send(new ProcessCloseTicketCommand(ticketId, user.Id, langData.GetTranslation(LangKeys.ChannelWasDeleted), args.Channel));
          Log.Information(logMessage, args.Channel.Id);
        }
      }
      catch (BotExceptionBase ex) {
        Log.Warning(ex, logMessage, args.Channel.Id);
      }
      catch (Exception ex) {
        Log.Error(ex, logMessage, args.Channel.Id);
      }
  }


  [PerformanceLoggerAspect]
  public static async Task ComponentInteractionCreated(DiscordClient client, ComponentInteractionCreatedEventArgs args) {
    const string logMessage = $"[{nameof(ComponentInteractionCreated)}]({{CustomId}},{{UserId}},{{ChannelId}},{{InteractionId}},{{MessageId}})";
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();

    try {
      await sender.Send(new UpdateDiscordUserCommand(args.User));
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

          await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, feedbackModal);
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
          await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage);
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          var selectedTypeKey = args.Values.FirstOrDefault();
          if (string.IsNullOrEmpty(selectedTypeKey)) break;

          await sender.Send(new ProcessChangeTicketTypeCommand(ticketId, selectedTypeKey, null, args.Channel, args.Message, args.User.Id));
          Log.Information(logMessage,
                          args.Interaction?.Data?.CustomId,
                          args.User?.Id,
                          args.Channel?.Id,
                          args.Interaction?.Id,
                          args.Message?.Id);
          break;
        }
        case "close_ticket": // This must stay due to deprecation and support for existing tickets (v2.0 beta)
        case "close_ticket_with_reason": {
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          var modal = Modals.CreateCloseTicketWithReasonModal(ticketId);
          await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);

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

  [PerformanceLoggerAspect]
  public static async Task ModalSubmitted(DiscordClient client, ModalSubmittedEventArgs args) {
    const string logMessage = $"[{nameof(ModalSubmitted)}]({{CustomId}},{{InteractionId}})";
    await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                                               new DiscordInteractionResponseBuilder().AsEphemeral().WithContent(LangProvider.This.GetTranslation(LangKeys.ThankYouForFeedback)));
    var scope = client.ServiceProvider.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    try {
      await sender.Send(new UpdateDiscordUserCommand(args.Interaction.User));

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

          var feedbackMessage = await args.Interaction.Channel.GetMessageAsync(feedbackMessageId);
          await sender.Send(new ProcessAddFeedbackCommand(ticketId, starCount, textInput, feedbackMessage));
          Log.Information(logMessage, args.Interaction.Data.CustomId, args.Interaction.Id);
          break;
        }
        case "close_ticket_with_reason": {
          var textInput = args.Values["reason"];
          var ticketIdParam = parameters[0];
          var ticketId = Guid.Parse(ticketIdParam);
          await sender.Send(new ProcessCloseTicketCommand(ticketId, args.Interaction.User.Id, textInput, args.Interaction.Channel));
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