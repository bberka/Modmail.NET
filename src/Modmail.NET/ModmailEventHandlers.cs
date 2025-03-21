using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Aspects;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Models.Dto;
using Modmail.NET.Queues;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET;

public class ModmailEventHandlers
{
  private readonly IServiceScopeFactory _scopeFactory;

  public ModmailEventHandlers(IServiceScopeFactory scopeFactory) {
    _scopeFactory = scopeFactory;
  }

  public async Task InteractionCreated(DiscordClient client, InteractionCreateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Interaction?.User));
  }

  public async Task OnUserUpdated(DiscordClient client, UserUpdateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.UserAfter));
  }

  public async Task OnUserSettingsUpdated(DiscordClient client, UserSettingsUpdateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.User));
  }

  public async Task OnThreadCreated(DiscordClient client, ThreadCreateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Thread?.CurrentMember?.Member));
  }

  public Task OnSocketError(DiscordClient client, SocketErrorEventArgs args) {
    Log.Error(args.Exception, "Socket error occured in {Client}", client.CurrentUser.Username);
    return Task.CompletedTask;
  }

  public async Task OnScheduledGuildEventUserRemoved(DiscordClient client, ScheduledGuildEventUserRemoveEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.User));
  }

  public async Task OnScheduledGuildEventUserAdded(DiscordClient client, ScheduledGuildEventUserAddEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.User));
  }

  public Task OnReady(DiscordClient client, ReadyEventArgs args) {
    Log.Information("Client is ready to process events");
    return Task.CompletedTask;
  }

  public async Task OnMessageDeleted(DiscordClient client, MessageDeleteEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Message.Author));
  }

  public async Task OnMessageReactionAdded(DiscordClient client, MessageReactionAddEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.User));
  }

  public async Task OnMessageReactionRemoved(DiscordClient client, MessageReactionRemoveEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.User));
  }

  public async Task OnMessageReactionRemovedEmoji(DiscordClient client, MessageReactionRemoveEmojiEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Message.Author));
  }

  public async Task OnMessageReactionsCleared(DiscordClient client, MessageReactionsClearEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Message.Author));
  }

  public async Task OnMessageUpdated(DiscordClient client, MessageUpdateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Author));
  }

  public async Task OnGuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Member));
  }

  public async Task OnGuildMemberRemoved(DiscordClient client, GuildMemberRemoveEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Member));
  }

  public Task OnHeartbeat(DiscordClient client, HeartbeatEventArgs args) {
    Log.Verbose("Heartbeat received from {Username}", client.CurrentUser.Username);
    return Task.CompletedTask;
  }

  public async Task OnMessageAcknowledged(DiscordClient client, MessageAcknowledgeEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args?.Message.Author));
  }

  public async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Message.Author));
    if (args.Message.Author.IsBot) return;
    if (args.Message.IsTTS) return;

    var ticketMessageQueue = scope.ServiceProvider.GetRequiredService<TicketMessageQueue>();
    await ticketMessageQueue.EnqueueMessage(args.Author.Id, new DiscordTicketMessageDto(client, args));
  }

  public Task OnClientError(DiscordClient client, ClientErrorEventArgs args) {
    Log.Error(args.Exception, "Exception occured in {Client}", client.CurrentUser.Username);
    return Task.CompletedTask;
  }

  public async Task OnGuildBanAdded(DiscordClient client, GuildBanAddEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Member));
  }

  public async Task OnGuildBanRemoved(DiscordClient client, GuildBanRemoveEventArgs args) {
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    await sender.Send(new UpdateDiscordUserCommand(args.Member));
  }

  [PerformanceLoggerAspect]
  public async Task OnChannelDeleted(DiscordClient client, ChannelDeleteEventArgs args) {
    const string logMessage = $"[{nameof(OnChannelDeleted)}]({{ChannelId}})";
    var scope = _scopeFactory.CreateScope();
    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    var langData = scope.ServiceProvider.GetRequiredService<LangProvider>();
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
    if (ticketId != Guid.Empty)
      try {
        var auditLogEntry = (await args.Guild.GetAuditLogsAsync(1, null, AuditLogActionType.ChannelDelete)).FirstOrDefault();
        var user = auditLogEntry?.UserResponsible ?? client.CurrentUser;
        await sender.Send(new UpdateDiscordUserCommand(user));
        var ticket = await sender.Send(new GetTicketQuery(ticketId));

        if (ticket.ClosedDateUtc.HasValue) return; // Ticket is already closed

        await sender.Send(new ProcessCloseTicketCommand(ticketId, user.Id, langData.GetTranslation(LangKeys.CHANNEL_WAS_DELETED), args.Channel));
        Log.Information(logMessage, args.Channel.Id);
      }
      catch (BotExceptionBase ex) {
        Log.Warning(ex, logMessage, args.Channel.Id);
      }
      catch (Exception ex) {
        Log.Error(ex, logMessage, args.Channel.Id);
      }
  }


  [PerformanceLoggerAspect]
  public async Task ComponentInteractionCreated(DiscordClient client, ComponentInteractionCreateEventArgs args) {
    const string logMessage = $"[{nameof(ComponentInteractionCreated)}]({{CustomId}},{{UserId}},{{ChannelId}},{{InteractionId}},{{MessageId}})";
    var scope = _scopeFactory.CreateScope();
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

          await sender.Send(new ProcessChangeTicketTypeCommand(ticketId, selectedTypeKey, null, args.Channel, args.Message, args.User.Id));
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
          await sender.Send(new ProcessCloseTicketCommand(ticketId, args.User.Id, null, args.Channel));
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

  [PerformanceLoggerAspect]
  public async Task ModalSubmitted(DiscordClient client, ModalSubmitEventArgs args) {
    const string logMessage = $"[{nameof(ModalSubmitted)}]({{CustomId}},{{InteractionId}})";
    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                                               new DiscordInteractionResponseBuilder().AsEphemeral().WithContent(LangProvider.This.GetTranslation(LangKeys.THANK_YOU_FOR_FEEDBACK)));
    var scope = _scopeFactory.CreateScope();
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