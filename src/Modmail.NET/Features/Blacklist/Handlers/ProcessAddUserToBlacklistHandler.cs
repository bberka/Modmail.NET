using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Blacklist.Commands;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class ProcessAddUserToBlacklistHandler : IRequestHandler<ProcessAddUserToBlacklistCommand, Database.Entities.Blacklist>
{
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessAddUserToBlacklistHandler(ModmailDbContext dbContext,
	                                        IMediator mediator) {
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async Task<Database.Entities.Blacklist> Handle(ProcessAddUserToBlacklistCommand request, CancellationToken cancellationToken) {
		var activeTicket = await _dbContext.Tickets.FilterActive().FirstOrDefaultAsync(cancellationToken);
		if (activeTicket is not null)
			await _mediator.Send(new ProcessCloseTicketCommand(activeTicket.Id,
			                                                   request.UserId,
			                                                   LangProvider.This.GetTranslation(Lang.TicketClosedDueToBlacklist),
			                                                   DontSendFeedbackMessage: true),
			                     cancellationToken);

		var activeBlock = await _dbContext.Blacklists.FilterByUserId(request.UserId).AnyAsync(cancellationToken);
		if (activeBlock) throw new ModmailBotException(Lang.UserAlreadyBlacklisted);

		var reason = string.IsNullOrEmpty(request.Reason)
			             ? LangProvider.This.GetTranslation(Lang.NoReasonProvided)
			             : request.Reason;

		var blackList = new Database.Entities.Blacklist {
			Reason = reason,
			UserId = request.UserId,
			AuthorUserId = request.AuthorizedUserId
		};

		_dbContext.Add(blackList);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();


		await _mediator.Publish(new NotifyBlockedUser(request.AuthorizedUserId, request.UserId, reason), cancellationToken);

		return blackList;
	}
}