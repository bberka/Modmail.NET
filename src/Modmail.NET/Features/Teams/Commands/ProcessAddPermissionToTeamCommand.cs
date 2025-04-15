using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessAddPermissionToTeamCommand(ulong AuthorizedUserId, Guid TeamId, AuthPolicy[] Policies) : IRequest<TeamPermission[]>,
                                                                                                                     IPermissionCheck;