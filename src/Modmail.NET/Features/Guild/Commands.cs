using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Guild;

public sealed record ClearGuildOptionCommand : IRequest;

public sealed record ProcessGuildSetupCommand(DiscordGuild Guild) : IRequest<GuildOption>;

public sealed record ProcessCreateLogChannelCommand(DiscordGuild Guild) : IRequest<DiscordChannel>;