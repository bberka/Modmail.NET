using DSharpPlus;
using DSharpPlus.Entities.AuditLogs;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Common.Aspects;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.User.Commands;
using Modmail.NET.Language;
using Serilog;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class OnChannelDeletedEvent
{
	[PerformanceLoggerAspect]
	public static async Task OnChannelDeleted(
		DiscordClient client,
		ChannelDeletedEventArgs args
	) {
		Log.Debug(
		          "[{Source}] Channel deletion event triggered. ChannelId: {ChannelId}",
		          nameof(OnChannelDeletedEvent),
		          args.Channel.Id
		         );

		using var scope = client.ServiceProvider.CreateScope();
		var sender = scope.ServiceProvider.GetRequiredService<ISender>();
		var langData = scope.ServiceProvider.GetRequiredService<LangProvider>();

		var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(args.Channel.Topic);
		if (ticketId == Guid.Empty) {
			Log.Debug(
			          "[{Source}] Channel is not a ticket channel, ignoring. ChannelId: {ChannelId}",
			          nameof(OnChannelDeletedEvent),
			          args.Channel.Id
			         );
			return;
		}

		try {
			var auditLogEntry = await args.Guild
			                              .GetAuditLogsAsync(1, null, DiscordAuditLogActionType.ChannelDelete)
			                              .FirstOrDefaultAsync();
			var user = auditLogEntry?.UserResponsible ?? client.CurrentUser;
			Log.Debug(
			          "[{Source}] Channel deletion initiated by user: {UserId}, ChannelId: {ChannelId}",
			          nameof(OnChannelDeletedEvent),
			          user.Id,
			          args.Channel.Id
			         );
			await sender.Send(new UpdateDiscordUserCommand(user));

			var dbContext = scope.ServiceProvider.GetRequiredService<ModmailDbContext>();
			var ticket = await dbContext.Tickets.FindAsync(ticketId);

			if (ticket is null) {
				Log.Warning(
				            "[{Source}] Could not retrieve ticket information, possibly already deleted. TicketId: {TicketId}, ChannelId: {ChannelId}",
				            nameof(OnChannelDeletedEvent),
				            ticketId,
				            args.Channel.Id
				           );
				return;
			}

			if (ticket.ClosedDateUtc.HasValue) {
				Log.Information(
				                "[{Source}] Ticket already closed, ignoring channel deletion event. TicketId: {TicketId}, ChannelId: {ChannelId}",
				                nameof(OnChannelDeletedEvent),
				                ticketId,
				                args.Channel.Id
				               );
				return; // Ticket is already closed
			}

			await sender.Send(new ProcessCloseTicketCommand(
			                                                user.Id,
			                                                ticketId,
			                                                langData.GetTranslation(Lang.ChannelWasDeleted)
			                                               ));

			Log.Information(
			                "[{Source}] Ticket channel deleted. TicketId: {TicketId}, ChannelId: {ChannelId}",
			                nameof(OnChannelDeletedEvent),
			                ticketId,
			                args.Channel.Id
			               );
		}
		catch (ModmailBotException ex) {
			Log.Warning(
			            ex,
			            "[{Source}] ModmailBotException: Error processing channel deletion. ChannelId: {ChannelId}",
			            nameof(OnChannelDeletedEvent),
			            args.Channel.Id
			           );
		}
		catch (Exception ex) {
			Log.Error(
			          ex,
			          "[{Source}] Unhandled exception processing channel deletion. ChannelId: {ChannelId}",
			          nameof(OnChannelDeletedEvent),
			          args.Channel.Id
			         );
		}
	}
}