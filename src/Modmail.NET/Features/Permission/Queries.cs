using MediatR;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Permission;

public sealed record GetPermissionLevelQuery(ulong UserId, bool IncludeRole = false) : IRequest<TeamPermissionLevel?>;

public sealed record GetPermissionInfoQuery : IRequest<PermissionInfo[]>;

public sealed record GetPermissionInfoOrHigherQuery(TeamPermissionLevel LevelOrHigher) : IRequest<PermissionInfo[]>;

public sealed record CheckUserInAnyTeamQuery(ulong MemberId) : IRequest<bool>;

public sealed record CheckRoleInAnyTeamQuery(ulong RoleId) : IRequest<bool>;

public sealed record CheckPermissionAccessQuery(ulong UserId, AuthPolicy Policy) : IRequest<bool>;