using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.UserInfo;

[CachePolicy("GetDiscordUserInfoQuery", 5)]
public sealed record GetDiscordUserInfoListQuery : IRequest<List<DiscordUserInfo>>;

[CachePolicy("GetDiscordUserInfoQuery", 5)]
public sealed record GetDiscordUserInfoDictQuery : IRequest<Dictionary<ulong, DiscordUserInfo>>;


[CachePolicy("GetDiscordUserInfoQuery", 60)]
public sealed record GetDiscordUserInfoQuery(ulong UserId) : IRequest<DiscordUserInfo>;