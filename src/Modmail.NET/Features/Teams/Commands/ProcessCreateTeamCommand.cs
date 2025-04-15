using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessCreateTeamCommand(
	ulong AuthorizedUserId,
	string TeamName,
	bool PingOnNewTicket = false,
	bool PingOnTicketMessage = false
) : IRequest<Team>,
    IPermissionCheck;