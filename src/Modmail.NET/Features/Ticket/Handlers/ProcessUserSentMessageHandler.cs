using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessUserSentMessageHandler : IRequestHandler<ProcessUserSentMessageCommand>
{
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessUserSentMessageHandler(ModmailDbContext dbContext,
	                                     IMediator mediator) {
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async ValueTask<Unit> Handle(ProcessUserSentMessageCommand request, CancellationToken cancellationToken) {
		var ticket = await _dbContext.Tickets
		                             .FilterActive()
		                             .FilterById(request.TicketId)
		                             .FirstOrDefaultAsync(cancellationToken) ?? throw new ModmailBotException(Lang.TicketNotFound);
		ticket.LastMessageDateUtc = UtilDate.GetNow();
		_dbContext.Update(ticket);
		var ticketMessage = TicketMessage.MapFrom(ticket.Id, request.Message, false);
		_dbContext.Add(ticketMessage);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(TicketConstants.ProcessedReactionDiscordEmojiUnicode));
		await Task.Delay(50, cancellationToken); //wait for privateChannel creation process to finish 	//TODO: Refactor this and handle it better
		await _mediator.Publish(new NotifyTicketMessageSent(ticket, ticketMessage), cancellationToken);
		return Unit.Value;
	}
}