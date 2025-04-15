using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.DiscordBot.Queries;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;
using NotFoundException = DSharpPlus.Exceptions.NotFoundException;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessChangePriorityHandler : IRequestHandler<ProcessChangePriorityCommand>
{
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessChangePriorityHandler(ISender sender,
	                                    ModmailDbContext dbContext,
	                                    ModmailBot bot) {
		_sender = sender;
		_dbContext = dbContext;
		_bot = bot;
	}

	public async Task Handle(ProcessChangePriorityCommand request, CancellationToken cancellationToken) {
		if (request.AuthorizedUserId == 0) throw new ModmailBotException(Lang.InvalidUser);

		var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
		ticket.ThrowIfNotOpen();

		var oldPriority = ticket.Priority;
		ticket.Priority = request.NewPriority;


		_dbContext.Update(ticket);
		await _dbContext.SaveChangesAsync(cancellationToken);

		_ = Task.Run(async () => {
			var modUser = await _sender.Send(new GetDiscordUserInfoQuery(request.AuthorizedUserId), cancellationToken);

			try {
				var guildOption = await _sender.Send(new GetOptionQuery(), cancellationToken);
				var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
				await privateChannel.SendMessageAsync(TicketBotMessages.User.TicketPriorityChanged(guildOption, modUser, ticket, oldPriority, request.NewPriority));
			}
			catch (NotFoundException) {
				//ignored
			}

			var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
			await logChannel.SendMessageAsync(LogBotMessages.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));

			try {
				var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
				var newChName = request.NewPriority switch {
					TicketPriority.Normal => TicketConstants.NormalPriorityEmoji + string.Format(TicketConstants.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
					TicketPriority.High => TicketConstants.HighPriorityEmoji + string.Format(TicketConstants.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
					TicketPriority.Low => TicketConstants.LowPriorityEmoji + string.Format(TicketConstants.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
					_ => ""
				};

				await ticketChannel.ModifyAsync(x => { x.Name = newChName; });
				await ticketChannel.SendMessageAsync(TicketBotMessages.Ticket.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));
			}
			catch (NotFoundException) {
				//ignored
			}
		}, cancellationToken);
	}
}