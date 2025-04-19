using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCloseTicketHandler : IRequestHandler<ProcessCloseTicketCommand>
{
	private readonly IMediator _mediator;
	private readonly ModmailDbContext _dbContext;

	public ProcessCloseTicketHandler(ModmailDbContext dbContext,
	                                 IMediator mediator) {
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async ValueTask<Unit> Handle(ProcessCloseTicketCommand request, CancellationToken cancellationToken) {
		var ticket = await _dbContext.Tickets
		                             .FilterActive()
		                             .FilterById(request.TicketId)
		                             .FirstOrDefaultAsync(cancellationToken);
		if (ticket is null) throw new ModmailBotException(Lang.TicketNotFound);
		var closeReason = request.CloseReason?.Trim();
		ticket.ClosedDateUtc = UtilDate.GetNow();
		ticket.CloseReason = string.IsNullOrEmpty(closeReason)
			                     ? null
			                     : closeReason;
		ticket.CloserUserId = request.AuthorizedUserId;
		_dbContext.Update(ticket);
		await _dbContext.SaveChangesAsync(cancellationToken);
		await _mediator.Publish(new NotifyTicketClosed(request.AuthorizedUserId, ticket, request.DontSendFeedbackMessage), cancellationToken);
		return Unit.Value;
	}
}