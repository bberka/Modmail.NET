using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Blacklist.Commands;
using Modmail.NET.Features.Blacklist.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Blacklist.Handlers;

public class ProcessRemoveUserFromBlacklistHandler : IRequestHandler<ProcessRemoveUserFromBlacklistCommand, Database.Entities.Blacklist>
{
	private readonly ModmailDbContext _dbContext;
	private readonly IMediator _mediator;

	public ProcessRemoveUserFromBlacklistHandler(ModmailDbContext dbContext,
	                                             IMediator mediator) {
		_dbContext = dbContext;
		_mediator = mediator;
	}

	public async ValueTask<Database.Entities.Blacklist> Handle(ProcessRemoveUserFromBlacklistCommand request, CancellationToken cancellationToken) {
		var blacklist = await _dbContext.Blacklists
		                                .FilterByUserId(request.UserId)
		                                .FirstOrDefaultAsync(cancellationToken);
		if (blacklist is null) throw new ModmailBotException(Lang.UserIsNotBlacklisted);
		_dbContext.Remove(blacklist);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		await _mediator.Publish(new NotifyUnblockedUser(request.AuthorizedUserId, request.UserId), cancellationToken);
		return blacklist;
	}
}