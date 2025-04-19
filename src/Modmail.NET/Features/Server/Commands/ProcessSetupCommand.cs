using DSharpPlus.Entities;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Server.Commands;

[RequireModmailPermission(nameof(AuthPolicy.SuperUser))]
public sealed record ProcessSetupCommand(ulong AuthorizedUserId, DiscordGuild Guild) : IRequest<Option>,
                                                                                       IPermissionCheck;