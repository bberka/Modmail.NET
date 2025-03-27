using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.Bot;

[CachePolicy("GetMainGuildQuery", 300)]
public sealed record GetDiscordMainGuildQuery : IRequest<DiscordGuild>;

[CachePolicy("GetDiscordLogChannelQuery", 60)]
public sealed record GetDiscordLogChannelQuery : IRequest<DiscordChannel>;

public sealed record GetDiscordMemberQuery(ulong UserId) : IRequest<DiscordMember>;