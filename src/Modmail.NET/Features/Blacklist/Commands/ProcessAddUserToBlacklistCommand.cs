using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Blacklist.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageBlacklist))]
public sealed record ProcessAddUserToBlacklistCommand(ulong AuthorizedUserId, ulong UserId, string? Reason = null) : IRequest<Database.Entities.Blacklist>,
                                                                                                                     IPermissionCheck;