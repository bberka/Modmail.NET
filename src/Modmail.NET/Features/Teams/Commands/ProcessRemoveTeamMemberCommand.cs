using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.Teams.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRemoveTeamMemberCommand(
  ulong AuthorizedUserId,
  ulong TeamMemberKey,
  TeamMemberDataType Type) : IRequest,
                             IPermissionCheck;