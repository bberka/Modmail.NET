using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessRenameTeamCommand(
  ulong AuthorizedUserId,
  Guid Id,
  string NewName) : IRequest,
                    IPermissionCheck;