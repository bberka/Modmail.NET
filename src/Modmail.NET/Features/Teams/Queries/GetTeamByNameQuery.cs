using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Teams.Queries;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record GetTeamByNameQuery(ulong AuthorizedUserId, string Name, bool AllowNull = false) : IRequest<GuildTeam>;