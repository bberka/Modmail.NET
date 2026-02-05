using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Blacklist.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageBlacklist))]
public sealed record ProcessRemoveUserFromBlacklistCommand(ulong AuthorizedUserId, ulong UserId)
    : IRequest<Database.Entities.Blacklist>, IPermissionCheck;