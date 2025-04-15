using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessCreateTeamHandler : IRequestHandler<ProcessCreateTeamCommand, Team>
{
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessCreateTeamHandler(ModmailDbContext dbContext,
	                                ISender sender) {
		_dbContext = dbContext;
		_sender = sender;
	}

	public async ValueTask<Team> Handle(ProcessCreateTeamCommand request, CancellationToken cancellationToken) {
		var exists = await _sender.Send(new CheckTeamExistsQuery(request.AuthorizedUserId, request.TeamName), cancellationToken);
		if (exists) throw new ModmailBotException(Lang.TeamWithSameNameAlreadyExists);

		var team = new Team {
			Name = request.TeamName,
			PingOnNewMessage = request.PingOnTicketMessage,
			PingOnNewTicket = request.PingOnNewTicket,
		};

		_dbContext.Add(team);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return team;
	}
}