using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Guild.Commands;

[PermissionCheck(nameof(AuthPolicy.Owner))]
public sealed record ProcessGuildSetupCommand(ulong AuthorizedUserId, DiscordGuild Guild) : IRequest<GuildOption>,
                                                                                            IPermissionCheck;