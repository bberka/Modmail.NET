using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessRenameTeamCommand(
  ulong AuthorizedUserId,
  Guid Id,
  string NewName) : IRequest,
                    IPermissionCheck;