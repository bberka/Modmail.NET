using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Teams;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record CheckTeamExistsQuery(ulong AuthorizedUserId, string Name) : IRequest<bool>,
                                                                                 IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record GetTeamListQuery(ulong AuthorizedUserId,bool ThrowIfEmpty = false) : IRequest<List<GuildTeam>>;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record GetTeamByNameQuery(ulong AuthorizedUserId,string Name, bool AllowNull = false) : IRequest<GuildTeam>;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record GetTeamQuery(ulong AuthorizedUserId,Guid Id, bool AllowNull = false) : IRequest<GuildTeam>;