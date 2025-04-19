using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessRemoveTeamCommand(
	ulong AuthorizedUserId,
	Guid Id) : IRequest<Team>,
	           IPermissionCheck;