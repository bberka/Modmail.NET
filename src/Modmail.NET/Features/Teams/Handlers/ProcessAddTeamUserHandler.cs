using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddTeamUserHandler : IRequestHandler<ProcessAddTeamUserCommand>
{
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessAddTeamUserHandler(ModmailDbContext dbContext,
	                                 ISender sender) {
		_dbContext = dbContext;
		_sender = sender;
	}

	public async ValueTask<Unit> Handle(ProcessAddTeamUserCommand request, CancellationToken cancellationToken) {
		var isUserAlreadyInTeam = await _sender.Send(new CheckUserInAnyTeamQuery(request.MemberId), cancellationToken);
		if (isUserAlreadyInTeam) throw new ModmailBotException(Lang.MemberAlreadyInTeam);

		var team = await _dbContext.Teams.FindAsync([request.Id], cancellationToken);
		if (team is null) throw new ModmailBotException(Lang.TeamNotFound);

		var memberEntity = new TeamUser {
			TeamId = team.Id,
			UserId = request.MemberId,
			RegisterDateUtc = UtilDate.GetNow()
		};

		_dbContext.Add(memberEntity);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return Unit.Value;
	}
}