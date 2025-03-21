using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Modmail.NET.Aspects;
using Modmail.NET.Attributes;
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
[SlashCommandGroup("ticket", "Ticket management commands.")]
[UpdateUserInformationForSlash]
[RequireMainServerForSlashCommand]
[RequireTicketChannelForSlash]
[ModuleLifespan(ModuleLifespan.Transient)]
public class TicketSlashCommands : ApplicationCommandModule
{
  private readonly ISender _sender;

  public TicketSlashCommands(ISender sender) {
    _sender = sender;
  }

  [SlashCommand("close", "Close a ticket.")]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason. User will be notified of this reason.")]
                                string reason = null) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(CloseTicket)}({{ContextUserId}},{{reason}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await _sender.Send(new GetTicketQuery(ticketId, MustBeOpen: true));
      var userId = ctx.User.Id;

      //allow only a team member or opener user
      if (ticket.OpenerUserId == userId) {
        var option = await _sender.Send(new GetGuildOptionQuery(false)) ?? throw new NullReferenceException();
        if (!option.AllowUsersToCloseTickets) {
          await ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                                                    Interactions.Error(LangKeys.YOU_DO_NOT_HAVE_PERMISSION_TO_USE_THIS_COMMAND.GetTranslation()).AsEphemeral());
          return;
        }
      }
      else {
        var isAnyTeamMember = await _sender.Send(new CheckUserInAnyTeamQuery(userId));
        if (!isAnyTeamMember) {
          await ctx.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
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

  [SlashCommand("set-priority", "Set the priority of a ticket.")]
  [RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Support)]
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetPriority)}({{ContextUserId}},{{priority}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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


  [SlashCommand("add-note", "Add a note to a ticket.")]
  [RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Support)]
  public async Task AddNote(InteractionContext ctx,
                            [Option("note", "Note to add")] string note) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(AddNote)}({{ContextUserId}},{{note}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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

  [SlashCommand("toggle-anonymous", "Toggle anonymous mode for a ticket.")]
  [RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Moderator)]
  public async Task ToggleAnonymous(InteractionContext ctx) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(ToggleAnonymous)}({{ContextUserId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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


  [SlashCommand("set-type", "Set the type of a ticket.")]
  [RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Support)]
  public async Task SetType(InteractionContext ctx,
                            [Option("type", "Type of the ticket")] [Autocomplete(typeof(TicketTypeProvider))]
                            string type) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetType)}({{ContextUserId}},{{type}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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


  [SlashCommand("get-type", "Gets the ticket type for the current ticket channel")]
  [RequirePermissionLevelOrHigherForSlash(TeamPermissionLevel.Support)]
  public async Task GetTicketType(InteractionContext ctx) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(GetTicketType)}()";
    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketType = await _sender.Send(new GetTicketTypeByChannelIdQuery(ctx.Channel.Id));
      await ctx.EditResponseAsync(Webhooks.Info(LangKeys.TICKET_TYPE.GetTranslation(), $"`{ticketType.Name}` - {ticketType.Description}"));
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