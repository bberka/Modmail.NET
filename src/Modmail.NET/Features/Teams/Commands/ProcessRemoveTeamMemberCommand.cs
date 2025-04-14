using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessRemoveTeamMemberCommand(
  ulong AuthorizedUserId,
  ulong UserId) : IRequest,
                  IPermissionCheck;