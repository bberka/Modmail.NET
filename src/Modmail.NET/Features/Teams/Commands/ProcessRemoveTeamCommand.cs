using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRemoveTeamCommand(
  ulong AuthorizedUserId,
  Guid Id) : IRequest<GuildTeam>,
             IPermissionCheck;