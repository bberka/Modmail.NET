using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessToggleAnonymousHandler : IRequestHandler<ProcessToggleAnonymousCommand>
{
	private readonly IMediator _mediator;
	private readonly ModmailDbContext _dbContext;

	public ProcessToggleAnonymousHandler(IMediator mediator,
	                                     ModmailDbContext dbContext) {
		_mediator = mediator;
		_dbContext = dbContext;
	}


	public async ValueTask<Unit> Handle(ProcessToggleAnonymousCommand request, CancellationToken cancellationToken) {
		var ticket = await _dbContext.Tickets
		                             .FilterActive()
		                             .FilterById(request.TicketId)
		                             .FirstOrDefaultAsync(cancellationToken);
		if (ticket is null) throw new ModmailBotException(Lang.TicketNotFound);
		ticket.Anonymous = !ticket.Anonymous;
		_dbContext.Update(ticket);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		await _mediator.Publish(new NotifyTicketAnonymousToggled(request.AuthorizedUserId, ticket), cancellationToken);
		return Unit.Value;
	}
}