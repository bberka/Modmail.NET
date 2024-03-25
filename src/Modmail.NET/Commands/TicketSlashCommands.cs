using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Modmail.NET.Attributes;
using Modmail.NET.Common;
using Modmail.NET.Entities;
using Modmail.NET.Providers;
using Modmail.NET.Static;
using Modmail.NET.Utils;

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
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      //The reason why this is before because channel will be deleted after calling ProcessCloseTicketAsync
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_CLOSED));
      await ticket.ProcessCloseTicketAsync(ctx.User.Id, reason, ctx.Channel);
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_NOT_FOUND));
    }
  }

  [SlashCommand("set-priority", "Set the priority of a ticket.")]
  public async Task SetPriority(InteractionContext ctx,
                                [Option("priority", "Priority of the ticket")]
                                TicketPriority priority) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      await ticket.ProcessChangePriority(ctx.User.Id, priority, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_PRIORITY_CHANGED));
      return;
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_NOT_FOUND));
    }
  }


  [SlashCommand("add-note", "Add a note to a ticket.")]
  public async Task AddNote(InteractionContext ctx,
                            [Option("note", "Note to add")] string note) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());


    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      await ticket.ProcessAddNoteAsync(ctx.User.Id, note);
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_NOT_FOUND));
    }
  }

  [SlashCommand("toggle-anonymous", "Toggle anonymous mode for a ticket.")]
  public async Task ToggleAnonymous(InteractionContext ctx) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      await ticket.ProcessToggleAnonymousAsync(ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_ANONYMOUS_TOGGLED));
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_NOT_FOUND));
    }
  }


  [SlashCommand("set-type", "Set the type of a ticket.")]
  public async Task SetType(InteractionContext ctx,
                            [Option("type", "Type of the ticket")] [Autocomplete(typeof(TicketTypeProvider))]
                            string type) {
    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    var ticket = await Ticket.GetActiveAsync(ticketId);
    if (ticket is not null) {
      await ticket.ProcessChangeTicketTypeAsync(ctx.User.Id, type, ctx.Channel);
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_TYPE_CHANGED));
    }
    else {
      //This should never happen
      await ctx.Interaction.EditOriginalResponseAsync(Webhooks.Success(Texts.TICKET_NOT_FOUND));
    }
  }
}