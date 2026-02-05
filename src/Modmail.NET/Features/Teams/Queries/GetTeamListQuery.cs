using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Teams.Queries;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record GetTeamListQuery(ulong AuthorizedUserId, bool ThrowIfEmpty = false) : IRequest<List<GuildTeam>>;