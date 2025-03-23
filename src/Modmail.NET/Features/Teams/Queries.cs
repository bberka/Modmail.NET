using MediatR;
using Modmail.NET.Entities;
using Modmail.NET.Models;

namespace Modmail.NET.Features.Teams;

public sealed record GetTeamPermissionLevelQuery(ulong UserId, ulong[] RoleIdList) : IRequest<TeamPermissionLevel?>;

public sealed record GetTeamPermissionInfoQuery : IRequest<List<PermissionInfo>>;

public sealed record GetPermissionInfoOrHigherQuery(TeamPermissionLevel LevelOrHigher) : IRequest<List<PermissionInfo>>;

public sealed record CheckUserInAnyTeamQuery(ulong MemberId) : IRequest<bool>;

public sealed record CheckRoleInAnyTeamQuery(ulong RoleId) : IRequest<bool>;

public sealed record CheckTeamExistsQuery(string Name) : IRequest<bool>;

public sealed record GetTeamListQuery(bool ThrowIfEmpty = false) : IRequest<List<GuildTeam>>;

public sealed record GetTeamByNameQuery(string Name, bool AllowNull = false) : IRequest<GuildTeam>;

public sealed record GetTeamQuery(Guid Id, bool AllowNull = false) : IRequest<GuildTeam>;