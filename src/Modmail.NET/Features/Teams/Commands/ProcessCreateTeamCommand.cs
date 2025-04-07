using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessCreateTeamCommand(
  ulong AuthorizedUserId,
  string TeamName,
  TeamPermissionLevel PermissionLevel,
  bool PingOnNewTicket = false,
  bool PingOnTicketMessage = false,
  bool AllowAccessToWebPanel = false
) : IRequest<GuildTeam>,
    IPermissionCheck;