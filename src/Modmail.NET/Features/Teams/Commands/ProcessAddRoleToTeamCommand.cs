using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record ProcessAddRoleToTeamCommand(
  ulong AuthorizedUserId,
  Guid Id,
  DiscordRole Role) : IRequest,
                      IPermissionCheck;