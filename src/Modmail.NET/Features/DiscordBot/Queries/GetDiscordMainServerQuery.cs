using DSharpPlus.Entities;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.DiscordBot.Queries;

[CachePolicy("GetMainGuildQuery", 300)]
public sealed record GetDiscordMainServerQuery : IRequest<DiscordGuild>;