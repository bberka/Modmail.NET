using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Teams.Commands;

[RequireModmailPermission(nameof(AuthPolicy.SuperUser))]
public sealed record SyncSuperUserTeamUsersCommand(ulong AuthorizedUserId) : IRequest, IPermissionCheck;