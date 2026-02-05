using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessUpdateTeamCommand(
    ulong AuthorizedUserId,
    Guid TeamId,
    string TeamName,
    bool? PingOnNewTicket,
    bool? PingOnTicketMessage) : IRequest, IPermissionCheck;