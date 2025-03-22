using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Aspects;
using Modmail.NET.Checks.Attributes;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Teams;
using Modmail.NET.Features.Ticket;
using Modmail.NET.Features.TicketType;
using Modmail.NET.Providers;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Commands.Slash;

[PerformanceLoggerAspect]
[Command("ticket")]
[Description("Ticket management commands.")]
[UpdateUserInformation]
[RequireMainServer]
[RequireTicketChannel]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Support)]
public class TicketSlashCommands 
{
  private readonly ISender _sender;

  public TicketSlashCommands(ISender sender) {
    _sender = sender;
  }

  [Command("close")]
  [Description("Close a ticket.")]
  public async Task CloseTicket(SlashCommandContext ctx,
                                [Parameter("reason")]
                                [Description("Ticket closing reason. User will be notified of this reason.")]
                                string reason = null) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(CloseTicket)}({{ContextUserId}},{{reason}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var isPrivateChannel = ctx.Channel.IsPrivate;
      var option = await _sender.Send(new GetGuildOptionQuery(false));
      var ticket = await _sender.Send(new GetTicketQuery(ticketId, MustBeOpen: true));
      if (ticket.OpenerUserId != ctx.User.Id) {
        var isAnyTeamMember = await _sender.Send(new CheckUserInAnyTeamQuery(ctx.User.Id));
        if (!isAnyTeamMember) {
          await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                                                    Interactions.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()).AsEphemeral());
          return;
        }
      }

      await _sender.Send(new ProcessCloseTicketCommand(ticketId, ctx.User.Id, reason, ctx.Channel));
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.TICKET_CLOSED.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id, reason);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, reason);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, reason);
    }
  }

  [Command("set-priority")]
  [Description("Set the priority of a ticket.")]

  public async Task SetPriority(SlashCommandContext ctx,
                                [Parameter("priority")]
                                [Description("Priority of the ticket")]
                                TicketPriority priority) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetPriority)}({{ContextUserId}},{{priority}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      await _sender.Send(new ProcessChangePriorityCommand(ticketId, ctx.User.Id, priority, ctx.Channel));
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.TICKET_PRIORITY_CHANGED.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id, priority);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, priority);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, priority);
    }
  }


  [Command("add-note")]
  [Description("Add a note to a ticket.")]
  public async Task AddNote(SlashCommandContext ctx,
                            [Parameter("note")] 
                            [Description("Note to add")] 
                            string note
    ) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(AddNote)}({{ContextUserId}},{{note}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      await _sender.Send(new ProcessAddNoteCommand(ticketId, ctx.User.Id, note));
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.NOTE_ADDED.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id, note);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id, note);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id, note);
    }
  }

  [Command("toggle-anonymous")]
  [Description("Toggle anonymous mode for a ticket.")]
  public async Task ToggleAnonymous(SlashCommandContext ctx) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(ToggleAnonymous)}({{ContextUserId}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      await _sender.Send(new ProcessToggleAnonymousCommand(ticketId, ctx.Channel));
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.TICKET_ANONYMOUS_TOGGLED.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id);
    }
    catch (BotExceptionBase ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage, ctx.User.Id);
    }
    catch (Exception ex) {
      await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage, ctx.User.Id);
    }
  }


  [Command("set-type")]
  [Description("Set the type of a ticket.")]
  public async Task SetType(SlashCommandContext ctx,
                            [Parameter("type")] 
                            [Description("Type of the ticket")] 
                            [SlashAutoCompleteProvider(typeof(TicketTypeProvider))]
                            string type) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetType)}({{ContextUserId}},{{type}})";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      await _sender.Send(new ProcessChangeTicketTypeCommand(ticketId, type, ctx.Channel, UserId: ctx.User.Id));
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(LangKeys.TICKET_TYPE_CHANGED.GetTranslation()));
      Log.Information(logMessage, ctx.User.Id, type);
    }
    catch (BotExceptionBase e) {
      await ctx.Interaction.EditOriginalResponseAsync(e.ToWebhookResponse());
      Log.Warning(e, logMessage, ctx.User.Id, type);
    }
    catch (Exception e) {
      await ctx.Interaction.EditOriginalResponseAsync(e.ToWebhookResponse());
      Log.Fatal(e, logMessage, ctx.User.Id, type);
    }
  }


  [Command("get-type")]
  [Description("Gets the ticket type for the current ticket channel")]
  public async Task GetTicketType(SlashCommandContext ctx) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(GetTicketType)}()";
    await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketType = await _sender.Send(new GetTicketTypeByChannelIdQuery(ctx.Channel.Id,true));
      if (ticketType is null) {
        await ctx.EditResponseAsync(Webhooks.Info(LangKeys.TICKET_TYPE_NOT_SET.GetTranslation()));
      }
      else {
        await ctx.EditResponseAsync(Webhooks.Info(LangKeys.TICKET_TYPE.GetTranslation(), $"`{ticketType.Name}` - {ticketType.Description}"));
      }
      Log.Information(logMessage);
    }
    catch (BotExceptionBase ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Warning(ex, logMessage);
    }
    catch (Exception ex) {
      await ctx.EditResponseAsync(ex.ToWebhookResponse());
      Log.Fatal(ex, logMessage);
    }
  }
}