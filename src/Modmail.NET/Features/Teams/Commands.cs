using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Teams;

public sealed record ProcessRenameTeamCommand(Guid Id, string NewName) : IRequest;

public sealed record ProcessRemoveTeamCommand(Guid Id) : IRequest<GuildTeam>;

public sealed record ProcessAddTeamMemberCommand(Guid Id, ulong MemberId) : IRequest;

public sealed record ProcessRemoveTeamMemberCommand(ulong TeamMemberKey, TeamMemberDataType Type) : IRequest;

public sealed record ProcessUpdateTeamCommand(
  string TeamName,
  TeamPermissionLevel? PermissionLevel,
  bool? PingOnNewTicket,
  bool? PingOnTicketMessage,
  bool? IsEnabled,
  bool? AllowAccessToWebPanel
) : IRequest;

public sealed record ProcessCreateTeamCommand(
  string TeamName,
  TeamPermissionLevel PermissionLevel,
  bool PingOnNewTicket = false,
  bool PingOnTicketMessage = false,
  bool AllowAccessToWebPanel = false
) : IRequest<GuildTeam>;

public sealed record ProcessAddRoleToTeamCommand(Guid Id, DiscordRole Role) : IRequest;