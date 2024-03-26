using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
using Modmail.NET.Providers;
using Modmail.NET.Static;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Commands;

[SlashCommandGroup("ticket", "Ticket management commands.")]
[UpdateUserInformation]
[RequirePermissionLevelOrHigher(TeamPermissionLevel.Moderator)]
[RequireMainServer]
[RequireTicketChannel]
public class TicketSlashCommands : ApplicationCommandModule
{
  [SlashCommand("close", "Close a ticket.")]
  public async Task CloseTicket(InteractionContext ctx,
                                [Option("reason", "Ticket closing reason")]
                                string? reason = null) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(CloseTicket)}({{ContextUserId}},{{reason}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessCloseTicketAsync(ctx.User.Id, reason, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_CLOSED));
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
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetPriority)}({{ContextUserId}},{{priority}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessChangePriority(ctx.User.Id, priority, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_PRIORITY_CHANGED));
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
  public async Task AddNote(InteractionContext ctx,
                            [Option("note", "Note to add")] string note) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(AddNote)}({{ContextUserId}},{{note}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessAddNoteAsync(ctx.User.Id, note);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.NOTE_ADDED));
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
  public async Task ToggleAnonymous(InteractionContext ctx) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(ToggleAnonymous)}({{ContextUserId}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessToggleAnonymousAsync(ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_ANONYMOUS_TOGGLED));
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
  public async Task SetType(InteractionContext ctx,
                            [Option("type", "Type of the ticket")] [Autocomplete(typeof(TicketTypeProvider))]
                            string type) {
    const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetType)}({{ContextUserId}},{{type}})";
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    try {
      var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
      var ticket = await Ticket.GetActiveTicketAsync(ticketId);
      await ticket.ProcessChangeTicketTypeAsync(ctx.User.Id, type, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_CHANGED));
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
}