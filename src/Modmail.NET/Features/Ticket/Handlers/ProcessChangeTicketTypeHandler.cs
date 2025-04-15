using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessChangeTicketTypeHandler : IRequestHandler<ProcessChangeTicketTypeCommand>
{
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessChangeTicketTypeHandler(ModmailDbContext dbContext,
	                                      IMediator mediator) {
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async ValueTask<Unit> Handle(ProcessChangeTicketTypeCommand request, CancellationToken cancellationToken) {
		//TODO: maybe add removal of embeds for the message to keep getting called if ticket is closed
		var ticket = await _dbContext.Tickets
		                             .FilterById(request.TicketId)
		                             .FilterActive()
		                             .FirstOrDefaultAsync(cancellationToken);
		if (ticket is null) throw new ModmailBotException(Lang.TicketNotFound);
		var ticketType = await _dbContext.TicketTypes.FilterByNameOrKey(request.Type).FirstOrDefaultAsync(cancellationToken);
		ticket.TicketTypeId = ticketType?.Id;
		_dbContext.Update(ticket);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		await _mediator.Publish(new NotifyTicketTypeChanged(request.AuthorizedUserId, ticket, ticketType), cancellationToken);
		return Unit.Value;
	}
}