using DSharpPlus.Entities;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.DiscordBot.Queries;

[CachePolicy("GetDiscordLogChannelQuery", 60)]
public sealed record GetDiscordLogChannelQuery : IRequest<DiscordChannel>;