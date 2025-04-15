using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessUpdateTeamHandler : IRequestHandler<ProcessUpdateTeamCommand>
{
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessUpdateTeamHandler(ModmailBot bot,
	                                ModmailDbContext dbContext,
	                                ISender sender) {
		_dbContext = dbContext;
		_sender = sender;
	}

	public async ValueTask<Unit> Handle(ProcessUpdateTeamCommand request, CancellationToken cancellationToken) {
		var anyChanges = request.PingOnNewTicket.HasValue || request.PingOnTicketMessage.HasValue;
		if (!anyChanges) return Unit.Value;

		var team = await _dbContext.Teams.FindAsync([request.TeamId], cancellationToken);
		if (team is null) throw new ModmailBotException(Lang.TeamNotFound);

		if (!string.IsNullOrEmpty(request.TeamName)) {
			var sameName = await _dbContext.Teams
			                               .AsNoTracking()
			                               .AnyAsync(x => x.Name == request.TeamName, cancellationToken);
			if (sameName) throw new ModmailBotException(Lang.TeamWithSameNameAlreadyExists);
			team.Name = request.TeamName;
		}

		if (request.PingOnNewTicket.HasValue) team.PingOnNewTicket = request.PingOnNewTicket.Value;

		if (request.PingOnTicketMessage.HasValue) team.PingOnNewMessage = request.PingOnTicketMessage.Value;

		_dbContext.Update(team);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return Unit.Value;
	}
}