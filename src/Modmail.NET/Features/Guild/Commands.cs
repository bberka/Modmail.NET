using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Guild;

[PermissionCheck(nameof(AuthPolicy.Owner))]
public sealed record ClearGuildOptionCommand(ulong AuthorizedUserId) : IRequest,
                                                                       IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.Owner))]
public sealed record ProcessGuildSetupCommand(ulong AuthorizedUserId, DiscordGuild Guild) : IRequest<GuildOption>,
                                                                                            IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.Owner))]
public sealed record ProcessCreateLogChannelCommand(ulong AuthorizedUserId, DiscordGuild Guild) : IRequest<DiscordChannel>,
                                                                                                  IPermissionCheck;