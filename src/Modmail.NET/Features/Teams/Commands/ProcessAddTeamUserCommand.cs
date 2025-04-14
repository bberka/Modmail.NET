using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessAddTeamUserCommand(
  ulong AuthorizedUserId,
  Guid Id,
  ulong MemberId) : IRequest,
                    IPermissionCheck;