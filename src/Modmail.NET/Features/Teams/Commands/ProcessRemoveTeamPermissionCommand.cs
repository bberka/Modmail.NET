using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageAccessPermissions))]
public sealed record ProcessRemoveTeamPermissionCommand(ulong AuthorizedUserId, AuthPolicy AuthPolicy) : IRequest<TeamPermission>,
                                                                                                         IPermissionCheck;