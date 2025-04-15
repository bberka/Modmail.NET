using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessAddNoteHandler : IRequestHandler<ProcessAddNoteCommand>
{
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessAddNoteHandler(ModmailBot bot,
	                             ModmailDbContext dbContext,
	                             IMediator mediator) {
		_bot = bot;
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async Task Handle(ProcessAddNoteCommand request, CancellationToken cancellationToken) {
		var ticket = await _dbContext.Tickets
		                             .FilterActive()
		                             .FilterById(request.TicketId)
		                             .FirstOrDefaultAsync(cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
		var noteEntity = new TicketNote {
			TicketId = ticket.Id,
			Content = request.Note,
			UserId = request.AuthorizedUserId
		};
		_dbContext.Add(noteEntity);
		await _dbContext.SaveChangesAsync(cancellationToken);
		await _mediator.Publish(new NotifyTicketNoteAdded(request.AuthorizedUserId, ticket, noteEntity), cancellationToken);
	}
}