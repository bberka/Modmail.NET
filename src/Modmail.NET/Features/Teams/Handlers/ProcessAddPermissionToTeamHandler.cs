using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Teams.Commands;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Teams.Handlers;

public class ProcessAddPermissionToTeamHandler : IRequestHandler<ProcessAddPermissionToTeamCommand, TeamPermission[]>
{
	private readonly ModmailDbContext _dbContext;
	private readonly IOptions<BotConfig> _options;
	private readonly ISender _sender;

	public ProcessAddPermissionToTeamHandler(ModmailDbContext dbContext,
	                                         IOptions<BotConfig> options,
	                                         ISender sender) {
		_dbContext = dbContext;
		_options = options;
		_sender = sender;
	}

	public async ValueTask<TeamPermission[]> Handle(ProcessAddPermissionToTeamCommand request, CancellationToken cancellationToken) {
		var team = await _dbContext.Teams
		                           .Where(x => x.Id == request.TeamId)
		                           .Include(x => x.Permissions)
		                           .Include(x => x.Users)
		                           .FirstOrDefaultAsync(cancellationToken); // Check if user trying to add permission is not in the team
		if (team is null) throw new ModmailBotException(Lang.TeamNotFound);
		if (team.SuperUserTeam) throw new InvalidOperationException();
		if (team.Users.Any(x => x.UserId == request.AuthorizedUserId)) throw new InvalidOperationException();
		var permissionsUserCanAssign = await GetPermissionsUserCanAssign(request.AuthorizedUserId, cancellationToken);
		var cleanedRequestedPermissions = GetCleanedRequestedPermissions(request.Policies, permissionsUserCanAssign, team.Permissions.Select(x => x.AuthPolicy).ToArray());

		var isAddingSuperUser = cleanedRequestedPermissions.Contains(AuthPolicy.SuperUser);
		if (isAddingSuperUser) {
			//Delete all other permissions
			_dbContext.TeamPermissions.RemoveRange(team.Permissions);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}

		var entities = cleanedRequestedPermissions.Select(x => new TeamPermission {
			AuthPolicy = x,
			TeamId = team.Id
		}).ToArray();
		_dbContext.TeamPermissions.AddRange(entities);
		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return entities;
	}

	private async Task<AuthPolicy[]> GetPermissionsUserCanAssign(ulong userId, CancellationToken cancellationToken) {
		var isConfigSuperUser = _options.Value.SuperUsers.Contains(userId);
		if (isConfigSuperUser) return AuthPolicy.List.ToArray();

		var userPermissions = await _sender.Send(new GetUserPermissionsQuery(userId), cancellationToken);
		if (userPermissions is null || userPermissions.Length == 0) throw new InvalidOperationException();

		var isSuperUser = userPermissions.Contains(AuthPolicy.SuperUser);
		if (isSuperUser) return AuthPolicy.List.Where(x => x != AuthPolicy.SuperUser).ToArray();

		return userPermissions;
	}

	private static AuthPolicy[] GetCleanedRequestedPermissions(AuthPolicy[] permissionsRequestedToAdd, AuthPolicy[] permissionsUserCanAssign, AuthPolicy[] permissionsAlreadyAssignedToTeam) {
		var hasSuperUser = permissionsRequestedToAdd.Any(x => x == AuthPolicy.SuperUser);
		if (hasSuperUser) return [AuthPolicy.SuperUser];

		var exceptAlreadyAssigned = permissionsRequestedToAdd.Except(permissionsAlreadyAssignedToTeam).ToArray();
		var onlyUserCanAssign = exceptAlreadyAssigned.Intersect(permissionsUserCanAssign).ToArray();
		if (onlyUserCanAssign.Length == 0) throw new ModmailBotException(Lang.NoPermissionsRequestedToAdd);

		return onlyUserCanAssign;
	}
}