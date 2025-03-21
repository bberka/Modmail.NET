using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.UserInfo;

public sealed record GetDiscordUserInfoListQuery : IRequest<List<DiscordUserInfo>>;

public sealed record GetDiscordUserInfoDictQuery : IRequest<Dictionary<ulong, DiscordUserInfo>>;

//TODO: Caching pipeline
public sealed record GetDiscordUserInfoQuery(ulong UserId) : IRequest<DiscordUserInfo>;