using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Server.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageOptions))]
public sealed record ClearOptionCommand(ulong AuthorizedUserId) : IRequest,
                                                                  IPermissionCheck;