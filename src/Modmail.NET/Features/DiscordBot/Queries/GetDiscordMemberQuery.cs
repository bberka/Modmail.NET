using DSharpPlus.Entities;
using Modmail.NET.Attributes;

namespace Modmail.NET.Features.DiscordBot.Queries;

[CachePolicy("GetDiscordMemberQuery", 5)]
public sealed record GetDiscordMemberQuery(ulong UserId) : IRequest<DiscordMember?>;