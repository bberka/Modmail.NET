using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;
using Modmail.NET.Features.DiscordCommands.Helpers;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordCommands.Handlers;

[PerformanceLoggerAspect]
[Command("ticket")]
[Description("Ticket management commands.")]
[UpdateUserInformation]
[RequireMainServer]
[RequireTicketChannel]
public class TicketSlashCommands
{
	private readonly ISender _sender;

	public TicketSlashCommands(ISender sender) {
		_sender = sender;
	}

	[Command("close")]
	[Description("Close a ticket.")]
	public async Task CloseTicket(SlashCommandContext ctx,
	                              [Parameter("reason")] [Description("Ticket closing reason. User will be notified of this reason.")]
	                              string? reason = null) {
		const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(CloseTicket)}({{ContextUserId}},{{reason}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
		try {
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
			var dbContext = ctx.ServiceProvider.GetRequiredService<ModmailDbContext>();
			var ticket = await dbContext.Tickets.FindAsync(ticketId) ?? throw new NullReferenceException(nameof(Ticket));
			ticket.ThrowIfNotOpen();

			if (ticket.OpenerUserId != ctx.User.Id) {
				var isAnyTeamMember = await _sender.Send(new CheckUserInAnyTeamQuery(ctx.User.Id));
				if (!isAnyTeamMember) {
					await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
					                                          ModmailInteractions.Error(Lang.YouDoNotHavePermissionToUseThisCommand.Translate()).AsEphemeral());
					return;
				}
			}

			await _sender.Send(new ProcessCloseTicketCommand(ctx.User.Id, ticketId, reason));
			await ctx.Interaction.EditOriginalResponseAsync(ModmailWebhooks.Success(Lang.TicketClosed.Translate()));
			Log.Information(logMessage, ctx.User.Id, reason);
		}
		catch (ModmailBotException ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Warning(ex, logMessage, ctx.User.Id, reason);
		}
		catch (Exception ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Fatal(ex, logMessage, ctx.User.Id, reason);
		}
	}

	[Command("priority")]
	[Description("Set the priority of a ticket.")]
	public async Task SetPriority(SlashCommandContext ctx,
	                              [Parameter("priority")] [Description("Priority of the ticket")]
	                              TicketPriority priority) {
		const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetPriority)}({{ContextUserId}},{{priority}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
		try {
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
			await _sender.Send(new ProcessChangePriorityCommand(ctx.User.Id, ticketId, priority, ctx.Channel));
			await ctx.Interaction.EditOriginalResponseAsync(ModmailWebhooks.Success(Lang.TicketPriorityChanged.Translate()));
			Log.Information(logMessage, ctx.User.Id, priority);
		}
		catch (ModmailBotException ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Warning(ex, logMessage, ctx.User.Id, priority);
		}
		catch (Exception ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Fatal(ex, logMessage, ctx.User.Id, priority);
		}
	}


	[Command("note")]
	[Description("Add a note to a ticket.")]
	public async Task AddNote(SlashCommandContext ctx,
	                          [Parameter("note")] [Description("Note to add")]
	                          string note
	) {
		const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(AddNote)}({{ContextUserId}},{{note}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
		try {
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
			await _sender.Send(new ProcessAddNoteCommand(ctx.User.Id, ticketId, note));
			await ctx.Interaction.EditOriginalResponseAsync(ModmailWebhooks.Success(Lang.NoteAdded.Translate()));
			Log.Information(logMessage, ctx.User.Id, note);
		}
		catch (ModmailBotException ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Warning(ex, logMessage, ctx.User.Id, note);
		}
		catch (Exception ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Fatal(ex, logMessage, ctx.User.Id, note);
		}
	}

	[Command("anonymous")]
	[Description("Toggle anonymous mode for a ticket.")]
	public async Task ToggleAnonymous(SlashCommandContext ctx) {
		const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(ToggleAnonymous)}({{ContextUserId}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
		try {
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
			await _sender.Send(new ProcessToggleAnonymousCommand(ctx.User.Id, ticketId));
			await ctx.Interaction.EditOriginalResponseAsync(ModmailWebhooks.Success(Lang.TicketAnonymousToggled.Translate()));
			Log.Information(logMessage, ctx.User.Id);
		}
		catch (ModmailBotException ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Warning(ex, logMessage, ctx.User.Id);
		}
		catch (Exception ex) {
			await ctx.Interaction.EditOriginalResponseAsync(ex.ToWebhookResponse());
			Log.Fatal(ex, logMessage, ctx.User.Id);
		}
	}


	[Command("type")]
	[Description("Change the type of a ticket.")]
	public async Task SetType(SlashCommandContext ctx,
	                          [Parameter("type")] [Description("Type of the ticket")] [SlashAutoCompleteProvider(typeof(TicketTypeProvider))]
	                          string type) {
		const string logMessage = $"[{nameof(TicketSlashCommands)}]{nameof(SetType)}({{ContextUserId}},{{type}})";
		await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
		try {
			var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
			await _sender.Send(new ProcessChangeTicketTypeCommand(ctx.User.Id, ticketId, type));
			await ctx.Interaction.EditOriginalResponseAsync(ModmailWebhooks.Success(Lang.TicketTypeChanged.Translate()));
			Log.Information(logMessage, ctx.User.Id, type);
		}
		catch (ModmailBotException e) {
			await ctx.Interaction.EditOriginalResponseAsync(e.ToWebhookResponse());
			Log.Warning(e, logMessage, ctx.User.Id, type);
		}
		catch (Exception e) {
			await ctx.Interaction.EditOriginalResponseAsync(e.ToWebhookResponse());
			Log.Fatal(e, logMessage, ctx.User.Id, type);
		}
	}
}