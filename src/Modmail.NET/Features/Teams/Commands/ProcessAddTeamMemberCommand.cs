using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessAddTeamMemberCommand(
  ulong AuthorizedUserId,
  Guid Id,
  ulong MemberId) : IRequest,
                    IPermissionCheck;