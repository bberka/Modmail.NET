using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessUpdateTeamCommand(
  ulong AuthorizedUserId,
  string TeamName,
  TeamPermissionLevel? PermissionLevel,
  bool? PingOnNewTicket,
  bool? PingOnTicketMessage,
  bool? IsEnabled,
  bool? AllowAccessToWebPanel
) : IRequest,
    IPermissionCheck;