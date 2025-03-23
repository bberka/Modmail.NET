using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Teams;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRenameTeamCommand(
  ulong AuthorizedUserId,
  Guid Id,
  string NewName) : IRequest,
                    IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRemoveTeamCommand(
  ulong AuthorizedUserId,
  Guid Id) : IRequest<GuildTeam>,
             IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessAddTeamMemberCommand(
  ulong AuthorizedUserId,
  Guid Id,
  ulong MemberId) : IRequest,
                    IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRemoveTeamMemberCommand(
  ulong AuthorizedUserId,
  ulong TeamMemberKey,
  TeamMemberDataType Type) : IRequest,
                             IPermissionCheck;

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

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessAddRoleToTeamCommand(
  ulong AuthorizedUserId,
  Guid Id,
  DiscordRole Role) : IRequest,
                      IPermissionCheck;