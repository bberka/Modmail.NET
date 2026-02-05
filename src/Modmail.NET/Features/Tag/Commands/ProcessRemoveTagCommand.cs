using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Tag.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTags))]
public sealed record ProcessRemoveTagCommand(ulong AuthorizedUserId, Guid Id) : IRequest<Database.Entities.Tag>, IPermissionCheck;